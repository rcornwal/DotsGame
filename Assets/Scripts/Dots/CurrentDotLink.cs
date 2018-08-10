using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentDotLink {

    public BoardSpace previousSpace { get; private set; }
    public BoardSpace lastConnected { get; private set; }

    List<BoardSpace> currentConnectedSpaces;
    List<BoardSpace> potentialSpaces;
    DotManager.DotType dotType;

    public CurrentDotLink() {
        currentConnectedSpaces = new List<BoardSpace>();
        potentialSpaces = new List<BoardSpace>();
    }

    public void Reset() {
        for (int i = 0; i < currentConnectedSpaces.Count; i++) {
            currentConnectedSpaces[i].RemoveAllConnected();
        }
        currentConnectedSpaces.Clear();
        potentialSpaces.Clear();
        previousSpace = null;
        lastConnected = null;
    }

    public bool ConnectionExists(BoardSpace space) {
        if (lastConnected != null) {
            if (lastConnected.IsConnected(space) || space.IsConnected(lastConnected)) {
                return true;
            }
        }
        return false;
    }

    public void AddSpace(BoardSpace space) {

        if (ConnectionExists(space)) {
            return;
        }

        previousSpace = lastConnected;
        lastConnected = space;

        if (IsEmpty()) {
            dotType = space.GetCurrentDot().GetDotType();
        }

        currentConnectedSpaces.Add(space);
        space.GetCurrentDot().Select();

        if (currentConnectedSpaces.Count >= 2) {
            previousSpace.Connect(lastConnected);
        }
    }

    public void RemoveSpace() {
        if (previousSpace != null) {
            previousSpace.RemoveLastConnected();
        }
        lastConnected = previousSpace;

        currentConnectedSpaces.RemoveAt(currentConnectedSpaces.Count - 1);

        if (currentConnectedSpaces.Count >= 2) {
            int lastIndex = currentConnectedSpaces.Count - 1;
            previousSpace = currentConnectedSpaces[lastIndex - 1];
        } else {
            previousSpace = null;
        }
    }

    public bool IsEmpty() {
        return currentConnectedSpaces.Count == 0;
    }

    public bool CreatesSquare(BoardSpace space) {
        if (ConnectionExists(space)) {
            return false;
        }
        return currentConnectedSpaces.Contains(space);
    }

    void AddPotentialSpace(BoardSpace space) {
        if (space == null || space.IsEmpty) {
            return;
        }
        if (space.GetCurrentDot().GetDotType().typeID != dotType.typeID) {
            return;
        }
        potentialSpaces.Add(space);
    }

    public List<BoardSpace> GetPotentialSpaces() {
        BoardSpace.AdjacentSpaces adjacentSpaces = lastConnected.GetAdjacentSpaces();
        potentialSpaces.Clear();
        AddPotentialSpace(adjacentSpaces.Top);
        AddPotentialSpace(adjacentSpaces.Bottom);
        AddPotentialSpace(adjacentSpaces.Left);
        AddPotentialSpace(adjacentSpaces.Right);
        return potentialSpaces;
    }

    public List<BoardSpace> GetConnectionSpaces() {
        return currentConnectedSpaces;
    }

    public DotManager.DotType GetDotType() {
        return dotType;
    }
}