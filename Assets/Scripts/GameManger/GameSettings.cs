using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General application settings, such as framerate, timescale, etc
/// </summary>
public class GameSettings : MonoBehaviour {

    [Header("GameSettings")]
    public int targetFrameRate;

	void Awake () {
        Application.targetFrameRate = targetFrameRate;
	}
}
