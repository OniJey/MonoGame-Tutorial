using System;

public static class GameMath
{
    public static float InverseClamp(float x, float min, float max)
    {
        if(max > x && x > min)
        {
            return x;
        } else
        {
            float differenceToMax = max - x;
            float differenceToMin = x - min;

            return (differenceToMin < differenceToMax)? min : max;
        }
    }
}