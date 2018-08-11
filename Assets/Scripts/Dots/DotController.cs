using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls the dot. Handles the drop movement and animations
/// </summary>
public class DotController : MonoBehaviour {

    public delegate void DropComplete();
    public DropComplete OnDropComplete;

    [Header("Child References")]
    public GameObject dotCircle;
    public GameObject selectionCircle;

    [Header("Settings")]
    public AnimationCurve dropCurve;

    // Components
    Animator animator;
    MovingObject movable;
    SpriteRenderer dotSprite;
    SpriteRenderer selectionSprite;

    DotManager.DotType dotType;
    List<Waypoint> dropWaypoints;
    public bool IsDropping { get; set; }

    [HideInInspector]
    public bool FlaggedToDrop;

    void Awake() {
        dotSprite = dotCircle.GetComponent<SpriteRenderer>();
        selectionSprite = selectionCircle.GetComponent<SpriteRenderer>();
        movable = GetComponent<MovingObject>();
        animator = GetComponent<Animator>();
    }

    void Start () {
        dropWaypoints = new List<Waypoint>();
        OnDropComplete = (() => { });
    }

    // Add waypoint to drop path
    public void AddWaypoint(Waypoint waypoint) {
        dropWaypoints.Add(waypoint);
    }

    // Stop and reset any movement
    public void StopMovement() {
        movable.Reset();
        dropWaypoints.Clear();
        FlaggedToDrop = false;
    }

    // Start the drop movement with the current dropWaypoints
    public void Drop(float dropTime) {
        dropWaypoints.Add(new Waypoint(transform.position));
        movable.AddWaypoints(dropWaypoints);
        movable.SetMoveTime(dropTime);
        movable.SetAnimationCurve(dropCurve);
        movable.OnMoveComplete = (()=>{
            IsDropping = false;
            dropWaypoints.Clear();
            OnDropComplete();
        });
        movable.Play();
    }

    // Set the type and change the colors
    public void SetDotType(DotManager.DotType type) {
        dotType = type;
        dotSprite.color = type.dotColor;
        selectionSprite.color = type.dotColor;
    }

    public DotManager.DotType GetDotType() {
        return dotType;
    }

    // Play the select animation
    public void Select() {
        animator.Play("DotSelect", 0);
    }

    // Play the score animation
    public void Score() {
        if (movable.IsMoving()) {
            movable.Reset();
        }
        animator.Play("DotScore", 0);
    }

    // Scale the dot sprite by the given scaleFactor, to match screen ratio
    public void SetScaleFactor(float scaleFactor) {
        Vector3 curScale = dotCircle.transform.localScale;
        Vector3 newScale = curScale * scaleFactor;
        dotCircle.transform.localScale = newScale;
    }

    // Reset dot and return to its object pool
    public void ScoreAnimationComplete() {
        PoolObject poolObject = GetComponent<PoolObject>();
        StopMovement();
        poolObject.ReturnToPool();
    }
}
