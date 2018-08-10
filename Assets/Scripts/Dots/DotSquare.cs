using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotSquare : MonoBehaviour {
    
    public Board board;

    List<BoardSpace> toScore;
    List<BoardSpace> connectedSpaces;
    bool squareCreated;

	// Use this for initialization
	void Start () {
        toScore = new List<BoardSpace>();
        connectedSpaces = new List<BoardSpace>();
	}
	
    public void Create(DotManager.DotType dotType) {        
        for (int i = 0; i < board.BoardArray.Count; i++) {
            for (int k = 0; k < board.BoardArray[i].Count; k++) {
                BoardSpace curSpace = board.BoardArray[i][k];
                DotController curDot = curSpace.GetCurrentDot();
                if (curDot.GetDotType().typeID == dotType.typeID) {
                    curDot.Select();
                    if (!squareCreated) {
                        toScore.Add(curSpace);
                    }
                }
            }
        }
        squareCreated = true;
        Handheld.Vibrate();
    }

    public void Score() {
        for (int i = 0; i < toScore.Count; i++) {
            toScore[i].GetCurrentDot().Score();
            toScore[i].SetEmpty(true);
        }
    }

    public bool SquareExists(CurrentDotLink dotLink) {
        List<BoardSpace> linkSpaces = dotLink.GetConnectionSpaces();
        for (int i = 0; i < linkSpaces.Count; i++) {
            List<BoardSpace> linkSpaceConnections = linkSpaces[i].GetConnectedSpaces();
            for (int k = 0; k < linkSpaceConnections.Count; k++) {
                if (connectedSpaces.Contains(linkSpaceConnections[k])) {
                    return true;
                }
                connectedSpaces.Add(linkSpaceConnections[k]);
            }
        }
        return false;
    }

    public void Reset() {
        squareCreated = false;
        connectedSpaces.Clear();
        toScore.Clear();
    }
}
