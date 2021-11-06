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

    //[Header("Minor Panels")]


    [Header("New Program Text Fields")]
    public Text SaveText;
    public Text TrainingText;
    public Text RoundsText;
    public Text SetsText;
    public Text ActiveText;
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
    public Text Finished_IntervalText;
    public Text Finished_TimeLeftText;


    [Header("Colors")]
    public Image TimerImage;
    public Color RestColor;
    public Color ActiveColor;
    public Color PrepColor;
    public Color DefaultColor;


    // training program data
    private float currentSets = 5;
    private float currentRounds = 5;
    private float currentActivePeriod = 30;
    private float currentRestPeriod = 20;
    private bool hasNewProgramBeenSaved = false;

    private int trainingTotalSeconds;
    private int trainingActiveSeconds;
    private int trainingMinutes;
    private int trainingSeconds;

    private int trainingPrepTime = 5;

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
        print("Canceling Current Training");
        HideAllScreens();
        OpeningScreen.SetActive(true);
        // and launch the timer
    }

    public void Start_Training()
    {
        print("Start Current Training!");
        // and launch the timer
    }
    #endregion

    #region Main Methods

    IEnumerator CountdownToStart()
    {

        TimerImage.color = PrepColor;

        while (trainingPrepTime > -1)
        {
            Training_TimeText.text = trainingPrepTime.ToString();
            trainingPrepTime--;

            yield return new WaitForSeconds(1);
        }
        TimerImage.color = ActiveColor;
        Training_TitleText.text = "Work!";
        StartCoroutine(Workout());
    }

    public IEnumerator Workout()
    {
        trainingActiveSeconds = trainingTotalSeconds;
        SetFinishedScreenText();
        while (trainingActiveSeconds != 0)
        {
            trainingActiveSeconds--;
            ProcessWorkoutTime(trainingActiveSeconds);
            TrainingSlider.value = Mathf.InverseLerp(0, trainingTotalSeconds, trainingActiveSeconds);

            // first comes the active period

            // then the rest

            yield return new WaitForSeconds(1);
        }

        GoTo_FinishedScreen();

    }

    private void SetFinishedScreenText()
    {
        Finished_SetText.text = currentSets.ToString() + " Sets";
        Finished_RoundText.text = currentRounds.ToString() + " Rounds Completed";
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


    private void CalculateTotalTime()
    {
        float setTime = currentActivePeriod + currentRestPeriod;
        trainingTotalSeconds = (int)((currentSets * currentRounds) * (setTime));

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
