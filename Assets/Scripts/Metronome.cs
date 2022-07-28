using UnityEngine;
using System.Collections;

public class Metronome : MonoBehaviour
{
    public static Metronome instance;
    public double bpm = 140.0F;

    double nextTick = 0.0F; // The next tick in dspTime
    double sampleRate = 0.0F;
    bool ticked = false;

    void Awake() { instance = this; }

    void Start()
    {
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;

        nextTick = startTick + (60.0 / bpm);
    }


    void LateUpdate()
    {

        if (!ticked && nextTick >= AudioSettings.dspTime)
        {
            ticked = true;
            GetComponent<AudioSource>().Play();
        }
    }

    // Just an example OnTick here


    void FixedUpdate()
    {
        double timePerTick = 60.0f / bpm;
        double dspTime = AudioSettings.dspTime;

        while (dspTime >= nextTick)
        {
            ticked = false;
            nextTick += timePerTick;
        }

    }
}