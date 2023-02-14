using System;
using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

namespace BeriUtils
{
    namespace Core
    {
        public static class BeriMath
        {
            ///<summary>
            ///Accelerate curSpeed to goalSpeed by accel * deltaTime ^ 2.
            ///</summary>
            public static float Accelerate(float curSpeed, float goalSpeed, float accel, float deltaTime)
            {
                float accelValue = accel * deltaTime * deltaTime;
                int sign = 1;
                if (goalSpeed < curSpeed) sign = -1;
                if (((sign * curSpeed) < sign * (goalSpeed + accelValue)) && (sign * curSpeed) > sign * (goalSpeed - accelValue))
                    return goalSpeed;

                return curSpeed + sign * accelValue;
            }
        }
        public class Timer
        {
            public float RemaingSeconds { get; private set; }


            public Timer(float duration)
            {
                RemaingSeconds = duration;
            }

            public event Action OnTimerStart;
            public event Action OnTimerEnd;
            // public event Action OnTimerStart; // <-- currently unused
            public void Tick(float deltaTime)
            {
                if (RemaingSeconds == 0f) return;
                //Debug.Log(RemaingSeconds);
                RemaingSeconds -= deltaTime;

                CheckForTimerEnd();
            }
            public void SetTimer(float value)
            {
                RemaingSeconds = value;
            }
            private void CheckForTimerEnd()
            {
                if (RemaingSeconds > 0f) { return; }
                RemaingSeconds = 0;
                OnTimerEnd?.Invoke();
            }
            public float GetRemaingingSeconds()
            {
                return RemaingSeconds;
            }
        }
    }
}
