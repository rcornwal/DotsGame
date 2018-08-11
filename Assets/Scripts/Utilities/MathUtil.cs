using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Math utility for custom equations and extention methods
/// </summary>
public static class MathUtil {
       
    // Given a value within rangeOne, returns the equivalent value within rangeTwo
    public static float ChangeRange(float value, Vector2 rangeOne, Vector2 rangeTwo){
        float oldMin = rangeOne [0];
        float oldMax = rangeOne [1];
        float newMin = rangeTwo [0];
        float newMax = rangeTwo [1];
        float oldRange = (oldMax - oldMin);
        float newRange = (newMax - newMin);
        float newValue = ((((value - oldMin) * newRange) / oldRange) + newMin);
        return newValue;
    }

}
