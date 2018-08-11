using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Moves an object along a straight line, according to its waypoint path and
/// animation curve.
/// </summary>
public class MovingObject : MonoBehaviour {

    public delegate void MoveComplete();
    public MoveComplete OnMoveComplete;

    bool isMoving;
    float moveTime;
	int startWaypointIndex;
    int endWaypointIndex;
    AnimationCurve animationCurve;
	List<Waypoint> waypointPath;

	IEnumerator MoveCoroutine;

    void Start() {
        waypointPath = new List<Waypoint>();
        OnMoveComplete += (() => { });
    }

    // Start moving along waypoint path
	public void Play() {        
        if (Setup()) {
            isMoving = true;
            StartCoroutine(MoveCoroutine);
        } else {
            FinishMovement();
        }
	}

    // Validate the moving object, and setup waypoints
    public bool Setup() {   

        // check for active game object
        if (!gameObject.activeSelf) {
            Debug.LogWarning("Attempt to move inactive gameobject");
            return false;
        }

        // check for empty waypoint list
        if (waypointPath.Count == 0) {
            Debug.LogWarning("Attempt to move without setting waypoints");
            return false;
        }

        // This assumes the dots always move in a straight line
        // we can ignore all waypoints between the first and last
        startWaypointIndex = 0;
        endWaypointIndex = waypointPath.Count - 1;;
        transform.position = waypointPath[0].Position;

        MoveCoroutine = MoveOverTime();
        return true;
    }

    // Set the waypoint path
    public void AddWaypoints(List<Waypoint> waypoints) {
        for (int i = 0; i < waypoints.Count; i++) {            
            waypointPath.Insert(0, waypoints[i]);
        }
    }

    // Move the object along its path, over the set amount of time
    IEnumerator MoveOverTime() {
        float curTime = 0;

        Vector3 startPos = waypointPath[startWaypointIndex].Position;
        Vector3 endPos = waypointPath[endWaypointIndex].Position;

        while (curTime < moveTime) {
            float positionPercent = curTime / moveTime;
            float positionT = positionPercent;
            if (animationCurve != null) {
                positionT = animationCurve.Evaluate(positionPercent);
            }

            Vector3 nextPos = Vector3.LerpUnclamped(startPos, endPos, positionT);
            transform.position = nextPos;
            curTime += Time.deltaTime;
            yield return 0;
        }
        FinishMovement();
    }

    // Movement has completed successful
    public void FinishMovement() {
        transform.position = waypointPath[endWaypointIndex].Position;
        OnMoveComplete();
        Reset();
    }

    // Set the curve the movement will follow
    public void SetAnimationCurve(AnimationCurve curve) {
        animationCurve = curve;
    }

    // Set how long the movement will take (in seconds)
    public void SetMoveTime(float newTime) {
        moveTime = newTime;
    }

    // Get if the object is currently moving
    public bool IsMoving() {
        return isMoving;
    }

    // Stop all movement, reset callbacks and waypoint path
    public void Reset() {
        StopAllCoroutines();
        animationCurve = null;
        waypointPath.Clear();
        OnMoveComplete += (() => { });
        isMoving = false;
    }
}
