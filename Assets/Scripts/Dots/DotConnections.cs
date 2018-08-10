using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotConnections : MonoBehaviour {

    [Header("References")]
    public DragInput dragInput;
    public GameObject connectionLinePrefab;
    public Board board;

    DotSquare dotSquare;
    CurrentDotLink currentDotLink;
    ConnectionLine touchLine;

    void Awake() {
        dotSquare = GetComponent<DotSquare>();    
    }

    void Start () {
        currentDotLink = new CurrentDotLink();
        dragInput.OnSwipe += OnSwipe;
        dragInput.OnTouchEnd += OnTouchEnd;
        touchLine = GameObject.Instantiate(connectionLinePrefab).GetComponent<ConnectionLine>();
	}

    void UpdateLine(Vector3 touchPos) {
        if (!currentDotLink.IsEmpty()) {
            BoardSpace lastConnected = currentDotLink.lastConnected;
            Vector3 lineStart = lastConnected.GetScreenPosition();

            touchLine.SetType(lastConnected.GetCurrentDot().GetDotType());
            touchLine.SetPoints(lineStart, touchPos);
        }
    }

    BoardSpace FindFirstSpace(Vector3 touchPos) {
        List<List<BoardSpace>> boardArray = board.BoardArray;
        for (int i = 0; i < boardArray.Count; i++) {
            for (int k = 0; k < boardArray[i].Count; k++) {
                BoardSpace curSpace = boardArray[i][k];
                float touchDist = Vector3.Distance(touchPos, curSpace.GetScreenPosition());
                if (touchDist < board.touchDistance) {
                    return curSpace;
                }
            }
        }
        return null;
    }

    BoardSpace FindSpace(Vector3 touchPos, List<BoardSpace> possibleSpaces) {
        for (int i = 0; i < possibleSpaces.Count; i++) {
            BoardSpace curSpace = possibleSpaces[i];
            float touchDist = Vector3.Distance(touchPos, curSpace.GetScreenPosition());
            if (touchDist < board.touchDistance) {
                return curSpace;
            }
        }
        return null;
    }

    void OnSwipe(Vector3 touchPos) {

        BoardSpace space;
        if (currentDotLink.IsEmpty()) {
            space = FindFirstSpace(touchPos);
        } else {
            List<BoardSpace> possibleSpaces = currentDotLink.GetPotentialSpaces();
            space = FindSpace(touchPos, possibleSpaces);
        }

        if (space != null) {
            if (space == currentDotLink.previousSpace) {
                currentDotLink.RemoveSpace();
            } else {
                if (currentDotLink.CreatesSquare(space)) {
                    dotSquare.Create(currentDotLink.GetDotType());
                }
                currentDotLink.AddSpace(space);
            }
        }
        UpdateLine(touchPos);
    }

    void OnTouchEnd() {
        if (dotSquare.SquareExists(currentDotLink)) {
            dotSquare.Score();
        } else {
            List<BoardSpace> connectionSpaces = currentDotLink.GetConnectionSpaces();
            if (connectionSpaces.Count > 1) {
                for (int i = 0; i < connectionSpaces.Count; i++) {
                    connectionSpaces[i].SetEmpty(true);
                    connectionSpaces[i].GetCurrentDot().Score();
                }
            }
        }
        dotSquare.Reset();
        currentDotLink.Reset();
        touchLine.Remove();
        board.DropDots();
    }
}
