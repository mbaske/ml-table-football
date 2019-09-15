using UnityEngine;

namespace TableFootball
{
    public class Timer
    {
        public bool IsRunning { get; private set; }
        public float EllapsedTotal => intervalSum + (IsRunning ? ellapsed : 0);

        float ellapsed => Time.time - startTime;

        float startTime;
        float intervalSum;

        public Timer()
        {
        }

        public void Reset()
        {
            intervalSum = 0;
            IsRunning = false;
        }

        public void StartInterval()
        {
            startTime = Time.time;
            IsRunning = true;
        }

        public void StopInterval()
        {
            intervalSum += ellapsed;
            IsRunning = false;
        }
    }
}