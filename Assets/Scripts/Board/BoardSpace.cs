using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Board space holds adjacent spaces, its current dot, and currently linked
/// spaces
/// </summary>
public class BoardSpace : MonoBehaviour {

    public bool IsEmpty { get; private set; }
    public bool IsTopSpace { get; private set; }

    Vector2Int boardIndex;
    Vector3 screenPosition;
    BoardCoordinateSpace coordinateSpace;
    DotController currentDot;
    DotManager dotManager;
    List<BoardSpace> connectedSpaces;
    BoardSpaceSpawner spawner;
    BoardSpaceLine connectionLines;

    // The board spaces adjacent to this one
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

    // Set the board coordinates to calulate this space's screen position
    public void SetCoordinateSpace(BoardCoordinateSpace boardCoordinateSpace) {
        coordinateSpace = boardCoordinateSpace;
    }

    // Set the dot manager
    public void SetDotManager(DotManager manager) {
        dotManager = manager;
    }
	
    // Set if the space is empty
    public void SetEmpty(bool isEmpty) {
        IsEmpty = isEmpty;
    }

    // Set the board space index
    public void SetBoardIndex(int x, int y) {
        boardIndex = new Vector2Int(x, y);
        CalculateScreenPosition();
    }

    // Get the screen position of this board space
    public Vector3 GetScreenPosition() {
        return screenPosition;
    }

    // Connect this board space with another, creating a line between them
    public void Connect(BoardSpace space) {
        Vector3 lineStart = GetScreenPosition();
        Vector3 lineEnd = space.GetScreenPosition();
        ConnectionLine line = connectionLines.GetUnusedLine();
        line.SetPoints(lineStart, lineEnd);
        connectedSpaces.Add(space);
    }

    // Remove the last connected board space
    public void RemoveLastConnected() {
        ConnectionLine line = connectionLines.GetLast();
        line.Remove();
        connectedSpaces.RemoveAt(connectedSpaces.Count - 1);
    }

    // Remove all connected board spaces
    public void RemoveAllConnected() {
        List<ConnectionLine> lines = connectionLines.GetAllLines();
        for (int k = 0; k < lines.Count; k++) {
            lines[k].Remove();
        }
        connectedSpaces.Clear();
    }

    // Return whether this board space is connected to the given space
    public bool IsConnected(BoardSpace space) {
        return connectedSpaces.Contains(space);
    }

    // Get all board spaces this one is connected to
    public List<BoardSpace> GetConnectedSpaces() {
        return connectedSpaces;
    }

    // Finds adjacent board spaces
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
            IsTopSpace = true;
            BoardSpaceSpawner boardSpaceSpawner = gameObject.AddComponent<BoardSpaceSpawner>();
            boardSpaceSpawner.SetCoordinateSpace(coordinateSpace);
            boardSpaceSpawner.SetDotManager(dotManager);
            spawner = boardSpaceSpawner;
        }
    }

    // Return the adjacent spaces
    public AdjacentSpaces GetAdjacentSpaces() {
        return adjacentSpaces;
    }

    // Return the dot currently on this board space
    public DotController GetCurrentDot() {
        return currentDot;
    }

    // Set the dot currently on this board space
    public void SetCurrentDot(DotController dot) {
        currentDot = dot;
    }

    // Get the dot spawner attached to this board space
    public BoardSpaceSpawner GetSpawner() {
        if (spawner == null) {
            Debug.LogWarning("Failed to find spawner");
        }
        return spawner;
    }

    // Place the board space in the correct grid position
    void CalculateScreenPosition() {
        float xSpacing = coordinateSpace.XSpacing();
        float ySpacing = coordinateSpace.YSpacing();
        float xOffset = xSpacing * boardIndex.x;
        float yOffset = ySpacing * boardIndex.y;
        float xPos = coordinateSpace.MinX() + xOffset + (xSpacing * .5f);
        float yPos = coordinateSpace.MinY() + yOffset + (ySpacing * .5f);
        screenPosition = new Vector3(xPos, yPos, 0);
        transform.position = screenPosition;
    }

    // Recusviely propogates up adjacent spaces to find a new dot
    DotController FindNextDot(BoardSpace space, List<Waypoint> waypoints) {
        AdjacentSpaces adjacent = space.GetAdjacentSpaces();
        Waypoint spaceWaypoint = new Waypoint(screenPosition);

        if (space.IsTopSpace) {
            BoardSpaceSpawner boardSpaceSpawner = space.GetSpawner();
            DotController dot = space.GetCurrentDot();

            // Spawn a new dot to drop
            if (space.IsEmpty || dot.FlaggedToDrop) {
                waypoints.Add(spaceWaypoint);
                DotController newDot = boardSpaceSpawner.SpawnDot(waypoints);
                newDot.FlaggedToDrop = true;
                return newDot;
            }

            // Current dot is valid
            waypoints.Add(spaceWaypoint);
            for (int i = 0; i < waypoints.Count; i++) {
                dot.AddWaypoint(waypoints[i]);
            }
            dot.FlaggedToDrop = true;
            return dot;
        }

        BoardSpace nextSpace = adjacent.Top;
        DotController nextDot = nextSpace.GetCurrentDot();

        // Propogate up adjacent dots
        if (nextSpace.IsEmpty || nextDot.FlaggedToDrop) {
            waypoints.Add(spaceWaypoint);
            return FindNextDot(nextSpace, waypoints);
        }

        // Found valid dot
        waypoints.Add(spaceWaypoint);
        for (int i = 0; i < waypoints.Count; i++) {
            nextDot.AddWaypoint(waypoints[i]);
        }
        nextDot.FlaggedToDrop = true;
        return nextDot;
    }

    // Find a new valid dot
    public void SetNewDot() {
        DotController newDot = FindNextDot(this, new List<Waypoint>());
        newDot.IsDropping = true;
        currentDot = newDot;
        connectionLines.SetDotType(newDot.GetDotType());
        IsEmpty = false;
    }

    // Start dropping current dot to this space's screen position
    public void DropDot(float dropSpeed) {
        if (currentDot.IsDropping) {
            currentDot.AddWaypoint(new Waypoint(screenPosition));
        }
        currentDot.Drop(dropSpeed);
    }
}
