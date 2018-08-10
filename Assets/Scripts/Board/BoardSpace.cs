using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace : MonoBehaviour {

    public bool IsEmpty { get; private set; }
    public bool IsTop { get; private set; }
    float xPos;
    float yPos;

    Vector2Int boardIndex;
    Vector3 screenPosition;
    BoardCoordinateSpace coordinateSpace;
    DotController currentDot;
    DotManager dotManager;
    List<BoardSpace> connectedSpaces;
    BoardSpaceLine connectionLines;

    [System.Serializable]
    public struct AdjacentSpaces{
        public BoardSpace Top;
        public BoardSpace Bottom;
        public BoardSpace Right;
        public BoardSpace Left;
        public AdjacentSpaces(BoardSpace _left, BoardSpace _right, 
                              BoardSpace _top, BoardSpace _bottom) {
            Left = _left;
            Right = _right;
            Top = _top;
            Bottom = _bottom;
        }
    }
    AdjacentSpaces adjacentSpaces;

	void Awake () {
        IsEmpty = true;
        connectionLines = GetComponent<BoardSpaceLine>();
	}

    void Start() {
        connectedSpaces = new List<BoardSpace>();
    }

    public void SetCoordinateSpace(BoardCoordinateSpace boardCoordinateSpace) {
        coordinateSpace = boardCoordinateSpace;
    }

    public void SetDotManager(DotManager manager) {
        dotManager = manager;
    }
	
    public void SetEmpty(bool isEmpty) {
        IsEmpty = isEmpty;
    }

    public void SetBoardIndex(int x, int y) {
        boardIndex = new Vector2Int(x, y);
        CalculateScreenPosition();
    }

    public Vector3 GetScreenPosition() {
        return screenPosition;
    }

    public void Connect(BoardSpace space) {
        Vector3 lineStart = GetScreenPosition();
        Vector3 lineEnd = space.GetScreenPosition();
        ConnectionLine line = connectionLines.GetUnusedLine();
        line.SetPoints(lineStart, lineEnd);
        connectedSpaces.Add(space);
    }

    public void RemoveLastConnected() {
        ConnectionLine line = connectionLines.GetLast();
        line.Remove();
        connectedSpaces.RemoveAt(connectedSpaces.Count - 1);
    }

    public void RemoveAllConnected() {
        List<ConnectionLine> lines = connectionLines.GetAllLines();
        for (int k = 0; k < lines.Count; k++) {
            lines[k].Remove();
        }
        connectedSpaces.Clear();
    }

    public bool IsConnected(BoardSpace space) {
        return connectedSpaces.Contains(space);
    }

    public List<BoardSpace> GetConnectedSpaces() {
        return connectedSpaces;
    }

    public void CalculateAdjacentSpaces(List<List<BoardSpace>> board) {
        BoardSpace left = null;
        BoardSpace right = null;
        BoardSpace top = null;
        BoardSpace bottom = null;
        if (boardIndex.x > 0) {
            left = board[boardIndex.x - 1][boardIndex.y];
        }
        if (boardIndex.x < board.Count - 1) {
            right = board[boardIndex.x + 1][boardIndex.y];
        }
        if (boardIndex.y > 0) {
            bottom = board[boardIndex.x][boardIndex.y - 1];
        }
        if (boardIndex.y < board[0].Count - 1) {
            top = board[boardIndex.x][boardIndex.y + 1];
        }
        adjacentSpaces = new AdjacentSpaces(left, right, top, bottom);

        // add dot spawners to the top row
        if (top == null) {
            IsTop = true;
            BoardSpaceSpawner spawner = gameObject.AddComponent<BoardSpaceSpawner>();
            spawner.SetCoordinateSpace(coordinateSpace);
            spawner.SetDotManager(dotManager);
        }
    }

    public AdjacentSpaces GetAdjacentSpaces() {
        return adjacentSpaces;
    }

    public DotController GetCurrentDot() {
        return currentDot;
    }

    public BoardSpaceSpawner GetSpawner() {
        BoardSpaceSpawner spawner = GetComponent<BoardSpaceSpawner>();
        if (spawner == null) {
            Debug.LogWarning("Failed to find spawner");
        }
        return spawner;
    }

    public void SetCurrentDot(DotController dot){
        currentDot = dot;
    }

    void CalculateScreenPosition() {
        float xSpacing = coordinateSpace.XSpacing();
        float ySpacing = coordinateSpace.YSpacing();
        float xOffset = xSpacing * boardIndex.x;
        float yOffset = ySpacing * boardIndex.y;
        xPos = coordinateSpace.MinX() + xOffset + (xSpacing * .5f);
        yPos = coordinateSpace.MinY() + yOffset + (ySpacing * .5f);
        screenPosition = new Vector3(xPos, yPos, 0);
        transform.position = screenPosition;
    }

    DotController FindNextDot(BoardSpace space, List<Waypoint> waypoints) {
        AdjacentSpaces adjacent = space.GetAdjacentSpaces();
        Waypoint spaceWaypoint = new Waypoint(screenPosition);

        if (space.IsTop) {
            BoardSpaceSpawner spawner = space.GetSpawner();
            DotController dot = space.GetCurrentDot();

            if (space.IsEmpty || dot.FlaggedToFall) {
                waypoints.Add(spaceWaypoint);
                DotController newDot = spawner.SpawnDot(waypoints);
                newDot.FlaggedToFall = true;
                return newDot;
            }

            waypoints.Add(spaceWaypoint);
            for (int i = 0; i < waypoints.Count; i++) {
                dot.AddWaypoint(waypoints[i]);
            }
            dot.FlaggedToFall = true;
            return dot;
        }

        BoardSpace nextSpace = adjacent.Top;
        DotController nextDot = nextSpace.GetCurrentDot();
        if (nextSpace.IsEmpty || nextDot.FlaggedToFall) {
            waypoints.Add(spaceWaypoint);
            return FindNextDot(nextSpace, waypoints);
        }

        waypoints.Add(spaceWaypoint);
        for (int i = 0; i < waypoints.Count; i++) {
            nextDot.AddWaypoint(waypoints[i]);
        }
        nextDot.FlaggedToFall = true;
        return nextDot;
    }

    public void SetNewDot() {
        DotController newDot = FindNextDot(this, new List<Waypoint>());
        newDot.IsDropping = true;
        currentDot = newDot;
        connectionLines.SetDotType(newDot.GetDotType());
        IsEmpty = false;
    }

    public void DropDot(float dropSpeed) {
        if (currentDot.IsDropping) {
            currentDot.AddWaypoint(new Waypoint(screenPosition));
        }
        currentDot.Drop(dropSpeed);
    }
}
