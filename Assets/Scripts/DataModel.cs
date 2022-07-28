using System.Collections.Generic;

[System.Serializable]
public class WorkoutData
{
    public float currentMetronome;
    public float currentWarmup;
    public float currentSets;
    public float currentRounds;
    public float currentActivePeriod;
    public float currentRestPeriod;
    public float currentRestRoundPeriod;

    public WorkoutData()
    {
        this.currentMetronome = 5;
        this.currentWarmup = 5;
        this.currentSets = 5;
        this.currentRounds = 5;
        this.currentActivePeriod = 30;
        this.currentRestPeriod = 20;
        this.currentRestRoundPeriod = 60;
    }


}

[System.Serializable]
public class MyWorkoutData
{
    public WorkoutData lastWorkout;

    public MyWorkoutData()
    {
        this.lastWorkout = new WorkoutData();
    }
}