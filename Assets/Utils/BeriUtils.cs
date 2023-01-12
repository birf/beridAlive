using UnityEngine;
namespace BeriUtils 
{
    public static class BeriMath 
    {
        ///<summary>
        ///Accelerate curSpeed to goalSpeed by accel * deltaTime ^ 2.
        ///</summary>
        public static float Accelerate(float curSpeed, float goalSpeed, float accel,float deltaTime)
        {
            float accelValue = accel*deltaTime*deltaTime;
            int sign = 1;
            if (goalSpeed < curSpeed) sign = -1;
            if (((sign*curSpeed) < sign *( goalSpeed + accelValue)) && (sign*curSpeed)>sign*(goalSpeed - accelValue))
            return goalSpeed;
            
            return curSpeed + sign * accelValue;
        }
    }
}
