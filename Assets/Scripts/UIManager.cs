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

    //[Header("Minor Panels")]


    [Header("Text Fields")]
    public Text SaveText;
    public Text TrainingText;
    public Text RoundsText;
    public Text SetsText;
    public Text ActiveText;

    public Text TotalText;


    // training program data
    private float currentSets = 1;
    private float currentRounds = 1;
    private float currentActivePeriod = 1;
    private float currentRestPeriod = 1;
    private bool hasNewProgramBeenSaved = false;



    // Start is called before the first frame update
    void Start()
    {
        GoTo_OpeningScreen();
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
        // and launch the timer
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

    public void Save_Training_Program()
    {
        if (!hasNewProgramBeenSaved)
        {
            print("Saving Program Data...");
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
        currentActivePeriod = active;
        ActiveText.text = active + " On / " + currentRestPeriod + " Off";
        CalculateTotalTime();
    }

    public void Set_Rest(float rest)
    {
        currentRestPeriod = rest;
        ActiveText.text = currentActivePeriod + " On / " + rest + " Off";
        CalculateTotalTime();
    }


    private void CalculateTotalTime()
    {
        TotalText.text = ((currentSets * currentRounds) * (currentActivePeriod + currentRestPeriod)).ToString() + " Minutes";
    }
    #endregion
}
