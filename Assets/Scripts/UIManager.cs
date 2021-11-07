using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main Screens")]
    public GameObject OpeningScreen;
    public GameObject NewScreen;
    public GameObject RepeatScreen;
    public GameObject SurpriseScreen;
    public GameObject TrainingScreen;
    public GameObject FinishedScreen;

    [Header("Minor Panels")]
    public GameObject RestRoundPanel;


    [Header("New Program Text Fields")]
    public Text SaveText;
    public Text TrainingText;
    public Text RoundsText;
    public Text SetsText;
    public Text ActiveText;
    public Text RestRoundText;
    public Text TotalText;

    [Header("Active Training ")]
    public Slider TrainingSlider;
    public Text Training_SetText;
    public Text Training_RoundText;
    public Text Training_IntervalText;
    public Text Training_TimeLeftText;
    public Text Training_TimeText;
    public Text Training_TitleText;


    [Header("Finished Training ")]
    public Text Finished_SetText;
    public Text Finished_RoundText;
    public Text Finished_TimeLeftText;


    [Header("Colors")]
    public Image TimerImage;
    public Color RestColor;
    public Color ActiveColor;
    public Color PrepColor;
    public Color DefaultColor;

    [Header("Audio Sources")]
    public AudioSource RoundBell;
    public AudioSource FinishBell;


    // training program data
    private float currentSets = 5;
    private float currentRounds = 5;
    private float currentActivePeriod = 30;
    private float currentRestPeriod = 20;
    private float currentRestRoundPeriod = 60;
    private bool hasNewProgramBeenSaved = false;

    private int trainingTotalSeconds;
    private int trainingActiveSeconds;
    private int trainingMinutes;
    private int trainingSeconds;

    private int trainingPrepTime = 5;

    int activeRounds;
    int activeSets;

    private void Awake()
    {
        //TODO: retrieve last saved work out

    }

    // Start is called before the first frame update
    void Start()
    {
        GoTo_OpeningScreen();
        CalculateTotalTime();
        // handle misc hides
        RestRoundPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI Methods

    private void HideAllScreens()
    {
        OpeningScreen.SetActive(false);
        NewScreen.SetActive(false);
        RepeatScreen.SetActive(false);
        SurpriseScreen.SetActive(false);
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

    public void GoTo_RepeatScreen()
    {
        HideAllScreens();
        RepeatScreen.SetActive(true);

    }

    public void GoTo_SurpriseScreen()
    {
        HideAllScreens();
        SurpriseScreen.SetActive(true);

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
    }

    public void Start_Training()
    {

        // and launch the timer
    }
    #endregion

    #region Main Methods

    IEnumerator CountdownToStart()
    {
        Training_TitleText.text = "Ready?";
        TimerImage.color = PrepColor;
        activeSets = (int)currentSets;
        activeRounds = (int)currentRounds;

        while (trainingPrepTime > -1)
        {
            Training_TimeText.text = trainingPrepTime.ToString();
            trainingPrepTime--;

            yield return new WaitForSeconds(1);
        }

        // store current workout program data for the finished screen
        SetFinishedScreenText();
        StartCoroutine(HandleWorkoutSlider());
        StartCoroutine(ActiveSet());
    }


    public IEnumerator HandleWorkoutSlider()
    {
        trainingActiveSeconds = trainingTotalSeconds;
        while (trainingActiveSeconds != 0)
        {
            trainingActiveSeconds--;
            ProcessWorkoutTime(trainingActiveSeconds);
            TrainingSlider.value = Mathf.InverseLerp(0, trainingTotalSeconds, trainingActiveSeconds);
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
        TimerImage.color = ActiveColor;
        Training_TitleText.text = "Work!";
        RoundBell.Play();
        while (activePeriod != 0)
        {
            activePeriod--;
            Training_TimeText.text = activePeriod.ToString();
            yield return new WaitForSeconds(1);
        }

        RoundBell.Play();

        if (activeSets > 1)
        {
            TimerImage.color = RestColor;
            Training_TitleText.text = "Rest!";
            while (restPeriod != 0)
            {
                restPeriod--;
                Training_TimeText.text = restPeriod.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        //we've finished a set
        activeSets--;
        Training_SetText.text = activeSets > 1 ? activeSets.ToString() + " Sets to Go!" : "Last Set!";

        // check to see if we have more sets to do
        if (activeSets != 0)
        {
            StartCoroutine(ActiveSet());
        }
        else//active sets equals zero which means we've finished a round
        {
            // fire off the Rest Round as long as it s not the last round
            if (activeRounds > 1)
            {
                RestRoundPanel.SetActive(true);
                TimerImage.color = RestColor;
                Training_TitleText.text = "Long Rest!";
                while (restRoundPeriod != 0)
                {
                    restRoundPeriod--;
                    Training_TimeText.text = restRoundPeriod.ToString();
                    yield return new WaitForSeconds(1);
                }
                RestRoundPanel.SetActive(false);
            }
            // we finally finished a full round
            activeRounds--;
            Training_RoundText.text = activeRounds > 1 ? activeRounds.ToString() + " Rounds Left!" : "Last Round!";
            if (activeRounds != 0)
            {
                // reset the set counter
                activeSets = (int)currentSets;
                //do it all again
                StartCoroutine(ActiveSet());
            }
        }
    }


    private void SetFinishedScreenText()
    {
        Finished_SetText.text = currentSets > 1 ? currentSets.ToString() + " Sets" : currentSets.ToString() + " Set";
        Finished_RoundText.text = currentRounds > 1 ? currentRounds.ToString() + " Rounds Completed" : currentRounds.ToString() + " Round Completed";
        Finished_TimeLeftText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds) + " Total Time";

    }

    public void Save_Training_Program()
    {
        if (!hasNewProgramBeenSaved)
        {
            print("Saving Program Data...");
            //TODO: actually save to disc for retrieval later when we select repeat last
            SaveText.text = "Start?";
            hasNewProgramBeenSaved = true;
        }
        else
            Launch_Training_Program();


    }

    public void Launch_Training_Program()
    {
        print("Launching Training Program...");
        GoTo_TrainingScreen();
        HandleTrainingTextUpdates();
        StartCoroutine(CountdownToStart());

    }

    #endregion


    #region Sliders

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
        float setTime = currentActivePeriod + currentRestPeriod;
        // add in the long rest for each round
        setTime = setTime + (currentRestRoundPeriod * currentRounds);
        // take off 1 rest period for each round as it is now a long rest...
        setTime = setTime - (currentRestPeriod * currentRounds);
        // figure out the raw total length based on sets and rounds
        trainingTotalSeconds = (int)((currentSets * currentRounds) * setTime);
        // now add in the long rest
        trainingTotalSeconds = (int)(trainingTotalSeconds + (currentRestRoundPeriod * currentRounds) - (currentRestPeriod * currentRounds));
        // finally take off the final long rest round at the end
        trainingTotalSeconds = (int)(trainingTotalSeconds - currentRestRoundPeriod);
        // handle the processing
        trainingMinutes = Mathf.FloorToInt(trainingTotalSeconds / 60F);
        trainingSeconds = Mathf.FloorToInt(trainingTotalSeconds - trainingMinutes * 60);
        TotalText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds);
    }

    private void ProcessWorkoutTime(int seconds)
    {
        trainingMinutes = Mathf.FloorToInt(seconds / 60F);
        trainingSeconds = Mathf.FloorToInt(seconds - trainingMinutes * 60);
        Training_TimeLeftText.text = string.Format("{0:00}:{1:00}", trainingMinutes, trainingSeconds);
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
