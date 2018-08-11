using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up the board coordinate space, and provides common board positions,
/// sizing, and spacing.
/// </summary>
public class BoardCoordinateSpace : MonoBehaviour {

    const float defaultScreenRatio = 1.5f;
    const float maxScreenRatio = 2.25f;

    [Header("Refereces")]
    public ScreenUtil screenUtil;

    [Header("Board Grid")]
    public int rows;
    public int columns;

    [Header("Dynamic Scaling")]
    public float boardSize;
    [Range(0, .5f)]
    public float sideBuffer;
    [Range(0, 1f)]
    public float minDotScale;

    float screenRatio;
    float boardExtent;
    Vector2 dotScaleRange;
    Vector2 screenRatioRange;

    void Awake() {
        screenRatio = screenUtil.GetScreenRatio();
        boardExtent = (boardSize * screenRatio);
        if (boardExtent > (1 - sideBuffer)) {
            boardExtent = 1 - sideBuffer;
        }
        dotScaleRange = new Vector2(1, minDotScale);
        screenRatioRange = new Vector2(defaultScreenRatio, maxScreenRatio);
    }

    public float MinX() {
        float centerX = screenUtil.CenterX();
        float width = screenUtil.Width() * .5f;
        return centerX - (width * boardExtent);
    }

    public float MaxX() {        
        float centerX = screenUtil.CenterX();
        float width = screenUtil.Width() * .5f;
        return centerX + (width * boardExtent);
    }

    public float MaxY() {            
        float centerY = screenUtil.CenterY();
        float height = MaxX() - MinX();
        return centerY + (height * .5f);
    }

    public float MinY() {            
        float centerY = screenUtil.CenterY();
        float height = MaxX() - MinX();
        return centerY - (height * .5f);
    }

    public float Height() {
        return (MaxY() + Mathf.Abs(MinY()));
    }

    public float Width() {
        return (MaxX() + Mathf.Abs(MinX()));
    }

    public int BoardWidth() {
        return rows;
    }

    public int BoardHeight() {
        return columns;
    }

    public float XSpacing() {
        return (Width() / (float)rows);
    }

    public float YSpacing() {
        return (Width() / (float)columns);
    }

    public float GetDotScaleFactor() {
        float scaleFactor = MathUtil.ChangeRange(screenRatio, screenRatioRange, dotScaleRange);
        return scaleFactor;
    }
}
