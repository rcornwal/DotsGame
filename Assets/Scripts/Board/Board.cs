using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    
    [Header("References")]
    public GameObject BoardSpacePrefab;
    public DotManager dotManager;

    [Header("Settings")]
    public float dropSpeed;
    public float dropRowDelay;
    public float touchDistance { get; private set; }
    public List<List<BoardSpace>> BoardArray { get; private set; }

    int boardWidth;
    int boardHeight;
    int dotsToDrop;
    int dotsDropped;

    BoardCoordinateSpace boardCoordinateSpace;
    WaitForSeconds dropRow;
    bool dotsDropping;
    Coroutine DropCoroutine;

    void Awake() {
        Application.targetFrameRate = 60;
        boardCoordinateSpace = GetComponent<BoardCoordinateSpace>();
    }

    IEnumerator TestDrop() {
        yield return new WaitForSeconds(.5f);
        DropDots();
    }

	// Use this for initialization
	void Start () {
        //Time.timeScale = .2f;
        boardWidth = boardCoordinateSpace.BoardWidth();
        boardHeight = boardCoordinateSpace.BoardHeight();
        dropRow = new WaitForSeconds(dropRowDelay);
        SetupBoard();
        touchDistance = Mathf.Min(boardCoordinateSpace.XSpacing(), boardCoordinateSpace.YSpacing()) * .5f;
        StartCoroutine(TestDrop());
	}
	
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

    void CalculateAdjacentSpaces() {
        for (int i = 0; i < boardWidth; i++) {
            for (int k = 0; k < boardHeight; k++) {
                BoardArray[i][k].CalculateAdjacentSpaces(BoardArray);
            }
        }
    }

    void OnDropComplete() {
        dotsDropped++;
        if (dotsDropped == dotsToDrop) {
            ResetSpawners();
            dotsDropping = false;
            dotsDropped = 0;
            dotsToDrop = 0;
        }
    }

    void ResetSpawners() {
        int topRow = boardHeight-1;
        for (int i = 0; i < BoardArray.Count; i++) {
            BoardSpaceSpawner spawner = BoardArray[i][topRow].GetComponent<BoardSpaceSpawner>();
            spawner.Reset();
        }
    }

    IEnumerator Drop() {
        bool overlappingDrop = dotsDropping;
        dotsDropping = true;
        for (int k = 0; k < boardHeight; k++) {
            for (int i = 0; i < boardWidth; i++) {
                BoardSpace curSpace = BoardArray[i][k];
                DotController curDot = curSpace.GetCurrentDot();
                bool alreadyDropping = curDot.IsDropping;
                if (curSpace.IsEmpty || curDot.FlaggedToFall || curDot.IsDropping) {
                    dotsToDrop++;
                    curDot.OnDropComplete += OnDropComplete;
                    curSpace.DropDot(dropSpeed);
                }
            }
            if (!overlappingDrop) {
                yield return dropRow;
            }
        }
    }

    public void DropDots() {
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

        for (int i = 0; i < boardWidth; i++) {
            for (int k = 0; k < boardHeight; k++) {
                BoardSpace curSpace = BoardArray[i][k];
                DotController curDot = curSpace.GetCurrentDot();
                if (curSpace.IsEmpty || curDot.FlaggedToFall) {
                    curSpace.SetNewDot();
                }
            }
        }
        DropCoroutine = StartCoroutine(Drop());
    }
}
