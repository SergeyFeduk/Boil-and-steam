using System;
using UnityEngine;

public class Timer 
{
    private float timeLeft;
    private float initialTime;
    private bool independentTimeScale;
    public Timer() { }
    public Timer(float time) {
        SetTime(time);
    }
    public Timer(float time, bool independent) {
        SetTime(time);
        MakeIndependent(independent);
    }

    #region Setters
    public void SetTime(float time) {
        timeLeft = time;
        initialTime = time;
    }

    public void SetFrequency(float time) {
        timeLeft = 1 / time;
    }

    public void MakeIndependent(bool independent) {
        independentTimeScale = independent;
    }
    #endregion

    public bool Execute() {
        if (timeLeft <= 0) return true;
        timeLeft -= independentTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        return false;
    }
    public bool ExecuteRoutine(Action<Timer> routine) {
        routine.Invoke(this);
        return !Execute();
    }

    #region Getters
    public float GetTimeLeft() {
        return timeLeft;
    }
    public float GetTimePassed() {
        return initialTime - timeLeft;
    }
    #endregion
}
