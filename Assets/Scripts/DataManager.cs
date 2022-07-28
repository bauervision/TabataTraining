using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    #region Essential Data Elements
    public MyWorkoutData data;
    public WorkoutData currentWorkoutData;

    #endregion


    void Start()
    {
        instance = this;

        if (DataSaver.CheckFirstTimeData())
        {
            LoadSavedData();


        }
        else//no saved data on the disc so create intial data
        {
            data = new MyWorkoutData();
            Debug.Log("No saved found, start fresh");

        }
    }

    private void LoadSavedData()
    {
        print("Loading saved data");
        //grab the complete data off the disc
        data = DataSaver.Load_Data();
        // add all the user data into list for easier access
        currentWorkoutData = data.lastWorkout;
        UIManager.instance.SetWorkOutDataFromPrevious(currentWorkoutData);

    }

    public void SetCurrentWorkout(WorkoutData newWorkout)
    {
        print("Saving Current Workout");
        data.lastWorkout = currentWorkoutData = newWorkout;
        DataSaver.Save_Data();
    }
















}
