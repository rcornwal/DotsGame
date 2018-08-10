using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenUtil : MonoBehaviour{
    
    Camera cam;
    Vector3 cameraCenter;
    Vector3 minScreenPoint;
    Vector3 maxScreenPoint;

    void Awake() {
        cam = GetComponent<Camera>();
        minScreenPoint = new Vector3(0, 0, 0);
        maxScreenPoint = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
        cameraCenter = cam.transform.position;
    }

    public float MinX() {
        return cam.ScreenToWorldPoint(minScreenPoint).x;
    }

    public float MinY() {
        return cameraCenter.y - cam.orthographicSize;
    }

    public float MaxX() {
        return cam.ScreenToWorldPoint(maxScreenPoint).x;
    }

    public float MaxY() {
        return cameraCenter.y + cam.orthographicSize;
    }

    public float CenterX() {
        return cameraCenter.x;
    }

    public float CenterY() {
        return cameraCenter.y;
    }

    public float Height() {
        return (MaxY() + Mathf.Abs(MinY()));
    }
               
    public float Width() {
        return (MaxX() + Mathf.Abs(MinX()));
    }

    public float GetScreenRatio() {
        float ratio =  ((float)Screen.height / (float)Screen.width);
        return ratio;
    }
}
