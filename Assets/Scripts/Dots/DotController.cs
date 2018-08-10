using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotController : MonoBehaviour {

    public delegate void DropComplete();
    public DropComplete OnDropComplete;

    [Header("Settings")]
    public AnimationCurve dropCurve;

    // Components
    Animator animator;
    SpriteRenderer dotCirlce;
    MovingObject movable;
    SpriteRenderer selectionCircle;

    DotManager.DotType dotType;
    DotManager dotManager;
    List<Waypoint> dropWaypoints;
    public bool IsDropping { get; set; }
    public bool FlaggedToFall;

    void Awake() {
        dotCirlce = transform.GetChild(0).GetComponent<SpriteRenderer>();
        selectionCircle = transform.GetChild(1).GetComponent<SpriteRenderer>();
        movable = GetComponent<MovingObject>();
        animator = GetComponent<Animator>();
    }

    void Start () {
        dropWaypoints = new List<Waypoint>();
        OnDropComplete = (() => { });
    }
	
    public void SetManager(DotManager manager) {
        dotManager = manager;
    }

    public DotManager GetManager() {
        return dotManager;
    }

    public void AddWaypoint(Waypoint waypoint) {
        dropWaypoints.Add(waypoint);
    }

    public void StopMovement() {
        movable.Reset();
        dropWaypoints.Clear();
        FlaggedToFall = false;
    }

    public void Drop(float dropSpeed) {
        dropWaypoints.Add(new Waypoint(transform.position));
        movable.AddWaypoints(dropWaypoints);
        movable.SetMoveSpeed(dropSpeed);
        movable.SetAnimationCurve(dropCurve);
        movable.OnMoveComplete = (()=>{
            IsDropping = false;
            dropWaypoints.Clear();
            OnDropComplete();
        });
        movable.Play();
    }

    public void SetDotType(DotManager.DotType type) {
        dotType = type;
        dotCirlce.color = type.dotColor;
        selectionCircle.color = type.dotColor;
    }

    public DotManager.DotType GetDotType() {
        return dotType;
    }

    public void Select() {
        animator.Play("DotSelect", 0);
    }

    public void Score() {
        if (movable.IsMoving()) {
            movable.Reset();
        }
        animator.Play("DotScore", 0);
    }

    public void ScoreAnimationComplete() {
        PoolObject poolObject = GetComponent<PoolObject>();
        if (poolObject == null) {
            Debug.LogError("Dot does not belong to an object pool");
            return;
        }
        StopMovement();
        poolObject.ReturnToPool();
    }
}
