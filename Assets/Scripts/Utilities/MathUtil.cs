using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil {
    
    public static float WrapAngle(float angle) {
        angle %= 360;
        if (angle > 180) {
            return angle - 360;
        }
        return angle;
    }

    public static float UnwrapAngle(float angle) {
        if (angle >= 0) {
            return angle;
        }
        angle = -angle % 360;
        return 360 - angle;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float tx = v.x;
        float ty = v.y;
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3){
        float u = 1 - t;
        float tt = t*t;
        float uu = u*u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * p0; 
        p += 3 * uu * t * p1; 
        p += 3 * u * tt * p2; 
        p += ttt * p3;      
        return p;
    }

    public static Vector3 DeCasteljau(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
        float oneMinusT = 1f - t;

        //Layer 1
        Vector3 Q = oneMinusT * p0 + t * p1;
        Vector3 R = oneMinusT * p1 + t * p2;
        Vector3 S = oneMinusT * p2 + t * p3;

        //Layer 2
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;

        //Final interpolated position
        Vector3 U = oneMinusT * P + t * T;

        return U;
    }

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

    // Convert our hex values to color objects
    public static Color hexToColor(string hex){
        hex = hex.Replace ("0x", "");    //in case the string is formatted 0xFFFFFF
        hex = hex.Replace ("#", "");    //in case the string is formatted #FFFFFF
        byte a = 255;                    //assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        if(hex.Length == 8){
            a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r,g,b,a);
    }

    public static float Clamp(float value, float min, float max) {
        if (value < min) {
            return min;
        }
        if (value > max) {
            return max;
        }
        return value;
    }

    public static Vector3 UiToWorld(GameObject obj) {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null) {
            Debug.LogError("Object is not a UI element");
            return Vector3.zero;
        }

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float xPos = Mathf.Lerp(corners[1].x, corners[2].x, .5f);
        float yPos = Mathf.Lerp(corners[0].y, corners[1].y, .5f);

        Vector3 worldPos = new Vector3(xPos, yPos, 0);
        return worldPos;
    }

    public static string KMBMaker(float num) {
        float numStr;
        string suffix;
        if (num < 1000f) {
            numStr = num;
            suffix = "";
            return Mathf.Floor(num).ToString();
        } else if (num < 1000000f) {
            numStr = num / 1000f;
            suffix = "K";
        } else if (num < 1000000000f) {
            numStr = num / 1000000f;
            suffix = "M";
        } else {
            numStr = num / 1000000000f;
            suffix = "B";
        }
        string kmb = numStr.ToString("n1");
        kmb = kmb.Contains(".") ? kmb.TrimEnd('0').TrimEnd('.') : kmb;
        return (kmb + suffix);
    }
}
