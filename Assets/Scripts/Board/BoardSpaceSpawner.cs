using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to the top board spaces. Spawns new dots to replenish the board 
/// </summary>
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
	
    // Board coordinate space, to get spacing between spawned dots
    public void SetCoordinateSpace(BoardCoordinateSpace boardCoordinateSpace) {
        coordinateSpace = boardCoordinateSpace;
    }

    // Set dot manager, to access the dot pool
    public void SetDotManager(DotManager manager) {
        dotManager = manager;
    }

    // Spawn a new dot
    public DotController SpawnDot(List<Waypoint> currentWaypoints) {

        DotController newDot = dotManager.GetNewDot();

        // add drop waypoints to the new dot
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
