using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionLine : MonoBehaviour {

    LineRenderer line;
    public bool InUse { get; private set; }

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
	}

    public void SetPoints(Vector3 start, Vector3 end) {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        InUse = true;
    }

    public void Remove() {
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);
        InUse = false;
    }

    public void SetType(DotManager.DotType dotType) {
        line.startColor = dotType.dotColor;
        line.endColor = dotType.dotColor;
    }
}
