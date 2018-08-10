using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceSpawner : MonoBehaviour {

    ScreenUtil screenUtil;
    DotManager dotManager;
    BoardCoordinateSpace coordinateSpace;
    List<DotController> dotsToDrop;

	// Use this for initialization
	void Start () {
        dotsToDrop = new List<DotController>();
        screenUtil = Camera.main.GetComponent<ScreenUtil>();
	}
	
    public void SetCoordinateSpace(BoardCoordinateSpace boardCoordinateSpace) {
        coordinateSpace = boardCoordinateSpace;
    }

    public void SetDotManager(DotManager manager) {
        dotManager = manager;
    }

    public DotController SpawnDot(List<Waypoint> currentWaypoints) {

        DotController newDot = dotManager.GetNewDot();

        // add waypoints to the new dot
        for (int i = 0; i < currentWaypoints.Count; i++) {
            newDot.AddWaypoint(currentWaypoints[i]);
        }
        for (int i = 0; i < dotsToDrop.Count; i++) {
            Waypoint waypoint = new Waypoint(dotsToDrop[i].transform.position);
            newDot.AddWaypoint(waypoint);
        }

        // place the spawned dot
        int dropNum = dotsToDrop.Count + 1;
        float screenTop = screenUtil.MaxY();
        float ySpacing = coordinateSpace.YSpacing();
        float xPos = transform.position.x;
        float yPos = screenTop + (ySpacing * dropNum);
        newDot.transform.position = new Vector3(xPos, yPos, 0);
        dotsToDrop.Add(newDot);
        return newDot;
    }

    public void Reset() {
        dotsToDrop.Clear();
    }
}
