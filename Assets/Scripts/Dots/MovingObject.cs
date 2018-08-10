using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public void SetMoveSpeed(float newTime){
        moveTime = newTime;
	}

	public void Play() {        
        if (Setup()) {
            isMoving = true;
            StartCoroutine(MoveCoroutine);
        } else {
            FinishMovement();
        }
	}

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
        startWaypointIndex = 0;
        endWaypointIndex = waypointPath.Count - 1;;
        transform.position = waypointPath[0].Position;

        MoveCoroutine = MoveTime();
        return true;
    }

    public void AddWaypoint(Waypoint waypoint) {
        waypointPath.Insert(0, waypoint);
    }

    public void AddWaypoints(List<Waypoint> waypoints) {
        float x = 0;
        for (int i = 0; i < waypoints.Count; i++) {
            if (i == 0) {
                x = waypoints[i].Position.x;
            }
            if (waypoints[i].Position.x != x) {
                Debug.LogError("Moving sideways");
            }
            waypointPath.Insert(0, waypoints[i]);
        }
    }

    IEnumerator MoveTime() {
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

    public void FinishMovement() {
        transform.position = waypointPath[endWaypointIndex].Position;
        OnMoveComplete();
        Reset();
    }

    public void SetAnimationCurve(AnimationCurve curve) {
        animationCurve = curve;
    }

	void Start() {
        waypointPath = new List<Waypoint>();
        OnMoveComplete += (() => { });
	}

    public bool IsMoving() {
        return isMoving;
    }

    public void Reset() {
        StopAllCoroutines();
        animationCurve = null;
        waypointPath.Clear();
        OnMoveComplete += (() => { });
        isMoving = false;
    }

	void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        for (int i = 0; i < waypointPath.Count; i++) {
            Vector3 point = waypointPath[i].Position;
            Gizmos.DrawSphere(point, .5f);
        }
	}
}
