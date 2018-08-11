using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets up the board by creating board spaces and handles dropping dots
/// to replenish the board
/// </summary>
public class Board : MonoBehaviour {
    
    [Header("References")]
    public GameObject BoardSpacePrefab;
    public DotManager dotManager;

    [Header("Settings")]
    public float dropTime;
    public float dropRowDelay;

    public float DotTouchDistance { get; private set; }
    public List<List<BoardSpace>> BoardArray { get; private set; }

    int boardWidth;
    int boardHeight;

    // used to calculate when dropping has finished
    int dotsToDrop;
    int dotsDropped;
    bool dotsDropping;

    BoardCoordinateSpace boardCoordinateSpace;
    WaitForSeconds dropRow;
    Coroutine DropCoroutine;

    void Awake() {
        Application.targetFrameRate = 60;
        boardCoordinateSpace = GetComponent<BoardCoordinateSpace>();
    }

    // Fills the board for the first time
    IEnumerator StartBoard() {
        yield return dropRow;
        DropDots();
    }

	// Use this for initialization
	void Start () {
        boardWidth = boardCoordinateSpace.BoardWidth();
        boardHeight = boardCoordinateSpace.BoardHeight();
        dropRow = new WaitForSeconds(dropRowDelay);
        SetupBoard();
        CalculateTouchRadius();
        StartCoroutine(StartBoard());
	}
	
    // Use the grid spacing to calculate the touch radius for each dot
    void CalculateTouchRadius() {
        float xSpacing = boardCoordinateSpace.XSpacing();
        float ySpacing = boardCoordinateSpace.YSpacing();
        DotTouchDistance = Mathf.Min(xSpacing, ySpacing) * .5f;
    }

    // Create our board array by instantiating board spaces
    void SetupBoard() {
        BoardArray = new List<List<BoardSpace>>();
        for (int i = 0; i < boardWidth; i++) {
            BoardArray.Add(new List<BoardSpace>());
            for (int k = 0; k < boardHeight; k++) {
                
                GameObject newBoardSpaceObject = GameObject.Instantiate(BoardSpacePrefab);
                newBoardSpaceObject.transform.SetParent(transform);

                BoardSpace newBoardSpace = newBoardSpaceObject.GetComponent<BoardSpace>();
                newBoardSpace.SetCoordinateSpace(boardCoordinateSpace);
                newBoardSpace.SetDotManager(dotManager);
                newBoardSpace.SetBoardIndex(i, k);
                BoardArray[i].Add(newBoardSpace);
            }
        }
        CalculateAdjacentSpaces();
    }

    // Call to each board space to find their adjacent spaces
    void CalculateAdjacentSpaces() {
        for (int i = 0; i < boardWidth; i++) {
            for (int k = 0; k < boardHeight; k++) {
                BoardArray[i][k].CalculateAdjacentSpaces(BoardArray);
            }
        }
    }

    // Callback for when each dot completes its drop movement
    void OnDropComplete() {
        dotsDropped++;
        if (dotsDropped == dotsToDrop) {
            ResetSpawners();
            dotsDropping = false;
            dotsDropped = 0;
            dotsToDrop = 0;
        }
    }

    // Clear out dot spawners at end of dropping
    void ResetSpawners() {
        int topRow = boardHeight-1;
        for (int i = 0; i < BoardArray.Count; i++) {
            BoardSpaceSpawner spawner = BoardArray[i][topRow].GetSpawner();
            spawner.Reset();
        }
    }

    IEnumerator Drop() {
        bool overlappingDrop = dotsDropping;
        dotsDropping = true;

        // Drop dots column by column
        for (int k = 0; k < boardHeight; k++) {
            for (int i = 0; i < boardWidth; i++) {
                BoardSpace curSpace = BoardArray[i][k];
                DotController curDot = curSpace.GetCurrentDot();
                bool alreadyDropping = curDot.IsDropping;
                if (curSpace.IsEmpty || curDot.FlaggedToDrop || curDot.IsDropping) {
                    dotsToDrop++;
                    curDot.OnDropComplete += OnDropComplete;
                    curSpace.DropDot(dropTime);
                }
            }

            // offset each row's drop, if its not an overlapping drop cal
            if (!overlappingDrop) {
                yield return dropRow;
            }
        }
    }

    // Drop dots to refill the board
    public void DropDots() {

        // stops all dot movement and drop coroutines, for overlapping drop calls
        if (DropCoroutine != null) {     
            StopCoroutine(DropCoroutine);
        }
        dotsToDrop = 0;
        dotsDropped = 0;
        for (int i = 0; i < boardWidth; i++) {
            for (int k = 0; k < boardHeight; k++) {
                BoardSpace curSpace = BoardArray[i][k];
                DotController curDot = curSpace.GetCurrentDot();
                if (!curSpace.IsEmpty) {
                    curDot.StopMovement();
                }
            }
        }

        // find or spawn new dots to go into score board spaces
        for (int i = 0; i < boardWidth; i++) {
            for (int k = 0; k < boardHeight; k++) {
                BoardSpace curSpace = BoardArray[i][k];
                DotController curDot = curSpace.GetCurrentDot();
                if (curSpace.IsEmpty || curDot.FlaggedToDrop) {
                    curSpace.SetNewDot();
                }
            }
        }
        DropCoroutine = StartCoroutine(Drop());
    }
}
