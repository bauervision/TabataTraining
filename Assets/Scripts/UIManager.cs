using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Main Screens")]
    public GameObject OpeningScreen;
    public GameObject NewScreen;
    public GameObject TrainingScreen;
    public GameObject FinishedScreen;

    [Header("Minor Panels")]
    public GameObject RestRoundPanel;


    [Header("New Program Text Fields")]
    public Text SaveText;
    public Text TrainingText;
    public Text RoundsText;
    public Text WarmupText;
    public Text MetronomeText;
    public Text SetsText;
    public Text ActiveText;
    public Text RestRoundText;
    //public Text TotalText;

    [Header("Active Training ")]
    public Slider TrainingSlider;
    public Text Training_SetText;
    public Text Training_RoundText;
    public Text Training_IntervalText;
    public Text Training_TimeLeftText;
    public TextMeshProUGUI Training_TimeText;
    public Text Training_TitleText;


    [Header("Finished Training ")]
    public Text Finished_SetText;
    public Text Finished_RoundText;
    public Text Finished_TimeLeftText;


    [Header("Colors")]

    public GameObject RestColor;
    public GameObject ActiveColor;
    public GameObject PrepColor;


    [Header("Audio Sources")]
    public AudioSource RoundBell;
    public AudioSource FinishBell;
    public AudioSource WarmupComplete;
    public AudioSource TenSeconds;
    public AudioSource LastRound;
    public AudioSource LastSet;
    public AudioSource BeginWarmup;

    [Header("Sliders")]
    public Slider WarmUpSlider;
    public Slider SetSlider;
    public Slider RoundSlider;
    public Slider OnSlider;
    public Slider OffSlider;
    public Slider RestSlider;
    public Slider MetronomeSlider;

    [Header("Metronome")]
    public Metronome metronome;

    // training program data
    private float currentMetronome = 5;
    private float currentWarmup = 5;//mins
    private float currentSets = 5;
    private float currentRounds = 5;
    private float currentActivePeriod = 30;
    private float currentRestPeriod = 20;
    private float currentRestRoundPeriod = 60;
    // private bool hasNewProgramBeenSaved = false;

    private int trainingTotalSeconds;
    private int trainingActiveSeconds;
    private int trainingMinutes;
    private int trainingSeconds;

    private int trainingPrepTime = 10;

    int activeRounds;
    int activeSets;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        GoTo_OpeningScreen();
        CalculateTotalTime();

        // handle misc hides
        RestRoundPanel.SetActive(false);
    }

    public void SetWorkOutDataFromPrevious(WorkoutData previous)
    {
        currentMetronome = previous.currentMetronome;
        currentWarmup = previous.currentWarmup;//mins
        currentSets = previous.currentSets;
        currentRounds = previous.currentRounds;
        currentActivePeriod = previous.currentActivePeriod;
        currentRestPeriod = previous.currentRestPeriod;
        currentRestRoundPeriod = previous.currentRestRoundPeriod;
        // now update all the sliders accordingly
        MetronomeSlider.value = (currentMetronome * 0.1f) - 9;
        WarmUpSlider.value = currentWarmup;
        SetSlider.value = currentSets;
        RoundSlider.value = currentRounds;
        OnSlider.value = currentActivePeriod / 10;
        OffSlider.value = currentRestPeriod / 10;
        RestSlider.value = currentRestRoundPeriod / 10;
    }

    #region UI Methods

    private void HideAllScreens()
    {
        OpeningScreen.SetActive(false);
        NewScreen.SetActive(false);
        TrainingScreen.SetActive(false);
        FinishedScreen.SetActive(false);
    }

    public void GoTo_OpeningScreen()
    {
        HideAllScreens();
        OpeningScreen.SetActive(true);

    }

    public void GoTo_NewScreen()
    {
        HideAllScreens();
        NewScreen.SetActive(true);

    }



    public void GoTo_TrainingScreen()
    {
        HideAllScreens();
        TrainingScreen.SetActive(true);
    }

    public void GoTo_FinishedScreen()
    {
        HideAllScreens();
        FinishedScreen.SetActive(true);
    }


    public void Cancel_Training()
    {
        HideAllScreens();
        OpeningScreen.SetActive(true);
        StopAllCoroutines();
        metronome.enabled = false;
    }


    #endregion

    #region Main Methods
    IEnumerator CountdownToStart(bool hadWarmup)
    {
        Training_TitleText.text = "Get Ready";

        PrepColor.SetActive(true);
        activeSets = (int)currentSets;
        activeRounds = (int)currentRounds;

        Training_SetText.text = "Working " + (activeSets > 1 ? activeSets.ToString() + " Sets" : "1 Set");
        Training_RoundText.text = "For " + (activeRounds > 1 ? activeRounds.ToString() + " Rounds" : "1 Round");
        Training_IntervalText.text = "On: " + currentActivePeriod.ToString() + " / Off " + currentRestPeriod.ToString();
        // reset the font size
        Training_TimeText.fontSize = 640;

        // if we didnt run a warmup
        if (!hadWarmup)
            TenSeconds.Play();

        while (trainingPrepTime > -1)
        {

            Training_TimeText.text = trainingPrepTime.ToString();
            Training_TimeText.color = Color.gray;
            trainingPrepTime--;

            yield return new WaitForSeconds(1);
        }

        SetFinishedScreenText();
        StartCoroutine(HandleWorkoutSlider());
        StartCoroutine(ActiveSet());
        trainingPrepTime = 10;
    }

    private void SaveCurrentWorkout()
    {
        // save the workout data
        WorkoutData newData = new WorkoutData();
        newData.currentActivePeriod = currentActivePeriod;
        newData.currentMetronome = currentMetronome;
        newData.currentRestPeriod = currentRestPeriod;
        newData.currentRestRoundPeriod = currentRestRoundPeriod;
        newData.currentRounds = currentRounds;
        newData.currentSets = currentSets;
        newData.currentWarmup = currentWarmup;
        DataManager.instance.SetCurrentWorkout(newData);
    }

    IEnumerator RunWarmup()
    {
        Training_TitleText.text = "Warm Up";
        PrepColor.SetActive(true);

        Training_SetText.text = "";
        Training_RoundText.text = "";
        Training_IntervalText.text = "";
        Training_TimeLeftText.text = "";
        // update warm up to minutes
        int totalWarmupTime = (int)currentWarmup * 60;
        currentWarmup = totalWarmupTime;
        // update the size for the warmup counter
        Training_TimeText.fontSize = 300;
        BeginWarmup.Play();

        while (currentWarmup > -1)
        {
            TrainingSlider.value = Mathf.InverseLerp(0, totalWarmupTime, currentWarmup);
            Training_TimeText.text = ProcessWarmupTime((int)currentWarmup);
            Training_TimeText.color = Color.gray;
            currentWarmup--;

            yield return new WaitForSeconds(1);
        }

        WarmupComplete.Play();

        // warm up complete fire off work out
        StartCoroutine(CountdownToStart(true));

    }

    private void HandlePrepColorChange()
    {
        RestColor.SetActive(false);
        ActiveColor.SetActive(false);
        PrepColor.SetActive(true);

    }

    private void HandleActiveColorChange()
    {
        RestColor.SetActive(false);
        ActiveColor.SetActive(true);
        PrepColor.SetActive(false);

    }

    private void HandleRestColorChange()
    {
        RestColor.SetActive(true);
        ActiveColor.SetActive(false);
        PrepColor.SetActive(false);
    }

    public IEnumerator HandleWorkoutSlider()
    {
        trainingActiveSeconds = trainingTotalSeconds;
        while (trainingActiveSeconds != 0)
        {
            ProcessWorkoutTime(trainingActiveSeconds);
            TrainingSlider.value = Mathf.InverseLerp(0, trainingTotalSeconds, trainingActiveSeconds);
            trainingActiveSeconds--;
            yield return new WaitForSeconds(1);
        }
        FinishBell.Play();
        GoTo_FinishedScreen();
        StopAllCoroutines();
    }

    public IEnumerator ActiveSet()
    {
        int activePeriod = (int)currentActivePeriod;
        int restPeriod = (int)currentRestPeriod;
        int restRoundPeriod = (int)currentRestRoundPeriod;
        HandleActiveColorChange();

        Training_TitleText.text = "Work!";
        Training_SetText.text = activeSets > 1 ? HandleActiveSetText() : "Last Set!";
        Training_RoundText.text = activeRounds > 1 ? activeRounds.ToString() + " Rounds Left" : "Last Round!";
        Training_IntervalText.text = currentActivePeriod.ToString() + " / " + currentRestPeriod.ToString();

        RoundBell.Play();

        if (currentMetronome != 0)
        {
            if (Metronome.instance.bpm != currentMetronome)
                Metronome.instance.bpm = currentMetronome;
            metronome.enabled = true;
        }


        while (activePeriod != 0)
        {
            Training_TimeText.text = activePeriod.ToString();
            Training_TimeText.color = Color.white;
            activePeriod--;

            // fire off a sound at 10 seconds
            if (activePeriod == 10)
            {
                TenSeconds.Play();
            }

            yield return new WaitForSeconds(1);
        }

        metronome.enabled = false;

        RoundBell.Play();

        if (activeSets > 1)
        {
            HandleRestColorChange();
            Training_TitleText.text = "Rest!";
            Training_SetText.text = activeSets > 2 ? (activeSets - 1).ToString() + " Sets Left" : "Almost Done!";
            while (restPeriod != 0)
            {

                if (restPeriod == 10 && (activeSets - 1) < 2)
                    LastSet.Play();

                Training_TimeText.text = restPeriod.ToString();
                Training_TimeText.color = Color.gray;
                restPeriod--;




                yield return new WaitForSeconds(1);
            }
        }

        //we've finished a set
        activeSets--;


        // check to see if we have more sets to do
        if (activeSets != 0)
            StartCoroutine(ActiveSet());
        else//active sets equals zero which means we've finished a round
        {
            // fire off the Rest Round as long as it s not the last round
            if (activeRounds > 1)
            {
                FinishBell.Play();
                RestRoundPanel.SetActive(true);
                HandleRestColorChange();
                Training_TitleText.text = "Long Rest!";
                while (restRoundPeriod != 0)
                {
                    Training_TimeText.text = restRoundPeriod.ToString();
                    Training_TimeText.color = Color.grey;
                    restRoundPeriod--;
                    Training_SetText.text = "Catch";
                    Training_RoundText.text = "Your";
                    Training_IntervalText.text = "Breath";

                    // play a final sound depending
                    if (restRoundPeriod == 10)
                    {
                        if ((activeRounds - 1) == 1)
                            LastRound.Play();
                    }
                    yield return new WaitForSeconds(1);
                }
                RestRoundPanel.SetActive(false);

            }
            // we finally finished a full round
            activeRounds--;
            if (activeRounds != 0)
            {
                // reset the set counter
                activeSets = (int)currentSets;
                //do it all again
                StartCoroutine(ActiveSet());
            }
        }
    }

    private string HandleActiveSetText()
    {
        switch (activeSets)
        {
            case 3: return "3rd Set";
            case 4: return "4th Set";
            case 5: return "5th Set";
            case 6: return "6th Set";
            case 7: return "7th Set";
            case 8: return "8th Set";
            case 9: return "9th Set";
            case 10: return "10th Set";
            default: return "2nd Set";
        }
    }

    private void SetFinishedScreenText()
    {
        string sets = currentSets > 1 ? currentSets.ToString() + " Sets" : currentSets.ToString() + " Set";
        string rounds = currentRounds > 1 ? currentRounds.ToString() + " Rounds" : currentRounds.ToString() + " Round";
        Finished_SetText.text = sets + " / " + rounds;
        // displays total time
        Finished_RoundText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds) + " Total Time";

        // now we need to subtract all of the rest times

        int totalRestEachRound = ((int)currentRestPeriod * ((int)currentSets - 1));
        int totalLongRestEachRound = ((int)currentRestRoundPeriod * ((int)currentRounds - 1));
        int totalRestTime = totalRestEachRound + totalLongRestEachRound;
        int finalTotalTimeSeconds = trainingTotalSeconds - totalRestTime;

        // handle the processing
        int finalActiveMinutes = Mathf.FloorToInt(finalTotalTimeSeconds / 60F);
        int finalActiveSeconds = Mathf.FloorToInt(finalTotalTimeSeconds - finalActiveMinutes * 60);
        // displays total active time
        Finished_TimeLeftText.text = string.Format("{0:00}:{1:00}", finalActiveMinutes, finalActiveSeconds) + " Total Active Time";

    }



    public void Launch_Training_Program()
    {
        GoTo_TrainingScreen();
        HandleTrainingTextUpdates();
        HandlePrepColorChange();
        SaveCurrentWorkout();
        // if we dont want a warm up, then launch the program
        if (currentWarmup == 0)
            StartCoroutine(CountdownToStart(false));
        else
            StartCoroutine(RunWarmup());

    }

    #endregion


    #region Sliders

    public void Set_Metronome(float rawValue)
    {
        // if anything other than zero, we want to use metronome
        if (rawValue != 0)
        {
            int startingBPS = 90;
            int interval = 10;
            currentMetronome = startingBPS + (interval * rawValue);
            MetronomeText.text = "Metronome: " + currentMetronome + " bps";
            MetronomeText.color = new Color(1, 0.8f, 0.4f, 1);
        }
        else
        {
            currentMetronome = 0;
            MetronomeText.text = "Metronome: Disabled";
            MetronomeText.color = Color.gray;
        }


    }

    public void Set_WarmUp(float mins)
    {
        if (mins != 0)
        {
            currentWarmup = mins;
            WarmupText.text = mins + " Min Warm Up";
            WarmupText.color = new Color(1, 0.8f, 0.4f, 1);
        }
        else
        {
            currentWarmup = 0;
            WarmupText.text = "Warm Up Disabled";
            WarmupText.color = Color.gray;
        }

    }

    public void Set_Sets(float sets)
    {
        currentSets = sets;
        SetsText.text = sets + " Sets";
        CalculateTotalTime();
    }

    public void Set_Rounds(float rounds)
    {
        currentRounds = rounds;
        RoundsText.text = rounds + " Rounds";
        CalculateTotalTime();
    }

    public void Set_Active(float active)
    {
        int activeRate = (int)active * 10;
        currentActivePeriod = activeRate;
        ActiveText.text = activeRate + " On / " + currentRestPeriod + " Off";
        CalculateTotalTime();
    }

    public void Set_Rest(float rest)
    {
        int restRate = (int)rest * 10;
        currentRestPeriod = restRate;
        ActiveText.text = currentActivePeriod + " On / " + restRate + " Off";
        CalculateTotalTime();
    }

    public void Set_RestRound(float rest)
    {
        int restRoundRate = (int)rest * 10;
        currentRestRoundPeriod = restRoundRate;
        RestRoundText.text = restRoundRate + " secs Rest Round";
        CalculateTotalTime();
    }


    private void CalculateTotalTime()
    {
        float singleSetTime = currentActivePeriod + currentRestPeriod;
        // remove 1 rest period for the long rest that replaces it after 1 full round
        float singleRoundTime = (singleSetTime * currentSets) - currentRestPeriod;
        // add in the one long rest
        singleRoundTime += currentRestRoundPeriod;

        // figure out the raw total length based on sets and rounds
        trainingTotalSeconds = (int)(currentRounds * singleRoundTime);
        // finally take off the final long rest round at the end
        trainingTotalSeconds = (int)(trainingTotalSeconds - currentRestRoundPeriod);
        // handle the processing
        trainingMinutes = Mathf.FloorToInt(trainingTotalSeconds / 60F);
        trainingSeconds = Mathf.FloorToInt(trainingTotalSeconds - trainingMinutes * 60);
        //TotalText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds);
    }

    private void ProcessWorkoutTime(int seconds)
    {
        trainingMinutes = Mathf.FloorToInt(seconds / 60F);
        trainingSeconds = Mathf.FloorToInt(seconds - trainingMinutes * 60);
        Training_TimeLeftText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds);
    }

    private string ProcessWarmupTime(int seconds)
    {
        trainingMinutes = Mathf.FloorToInt(seconds / 60F);
        trainingSeconds = Mathf.FloorToInt(seconds - trainingMinutes * 60);
        return string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds);
    }

    private void HandleTrainingTextUpdates()
    {
        Training_SetText.text = currentSets.ToString() + " Sets to Go";
        Training_RoundText.text = currentRounds.ToString() + " Rounds Left";
        Training_IntervalText.text = currentActivePeriod.ToString() + " / " + currentRestPeriod.ToString();
        Training_TimeLeftText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds);
    }
    #endregion
}
