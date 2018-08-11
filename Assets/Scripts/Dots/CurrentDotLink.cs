using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The current link of connected dots
/// </summary>
public class CurrentDotLink {

    public BoardSpace lastConnected { get; private set; }
    public BoardSpace previousConnected { get; private set; }

    List<BoardSpace> currentConnectedSpaces;
    List<BoardSpace> potentialSpaces;
    DotManager.DotType dotType;

    public CurrentDotLink() {
        currentConnectedSpaces = new List<BoardSpace>();
        potentialSpaces = new List<BoardSpace>();
    }

    // Check if the given space is already connected to the last; prevents loops
    public bool ConnectionExists(BoardSpace space) {
        if (lastConnected != null) {
            if (lastConnected.IsConnected(space) || space.IsConnected(lastConnected)) {
                return true;
            }
        }
        return false;
    }

    // Attempt to add the given space to the current dot link
    public void AddSpace(BoardSpace space) {

        // already connected, don't allow
        if (ConnectionExists(space)) {
            return;
        }

        previousConnected = lastConnected;
        lastConnected = space;

        // the link's type matches the first dot added
        if (IsEmpty()) {
            dotType = space.GetCurrentDot().GetDotType();
        }

        // add the dot
        currentConnectedSpaces.Add(space);
        space.GetCurrentDot().Select();

        // connect last spaces with the line
        if (currentConnectedSpaces.Count >= 2) {
            previousConnected.Connect(lastConnected);
        }
    }

    // Removes last added space from the link
    public void RemoveSpace() {

        // removes last created line
        if (previousConnected != null) {
            previousConnected.RemoveLastConnected();
        }
        lastConnected = previousConnected;

        currentConnectedSpaces.RemoveAt(currentConnectedSpaces.Count - 1);

        // update references to the lastConnected, and previously connected spaces
        if (currentConnectedSpaces.Count >= 2) {
            int lastIndex = currentConnectedSpaces.Count - 1;
            previousConnected = currentConnectedSpaces[lastIndex - 1];
        } else {
            previousConnected = null;
        }
    }

    public bool IsEmpty() {
        return currentConnectedSpaces.Count == 0;
    }

    // Check if adding the given space will create a square
    public bool CreatesSquare(BoardSpace space) {
        if (ConnectionExists(space)) {
            return false;
        }
        return currentConnectedSpaces.Contains(space);
    }

    // Check if the given space is valid, and of the same type
    void AddPotentialSpace(BoardSpace space) {
        if (space == null || space.IsEmpty) {
            return;
        }
        if (space.GetCurrentDot().GetDotType().typeID != dotType.typeID) {
            return;
        }
        potentialSpaces.Add(space);
    }

    // Get list of next spaces we could add to the link
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

    // Empty and reset our dot link
    public void Reset() {
        for (int i = 0; i < currentConnectedSpaces.Count; i++) {
            currentConnectedSpaces[i].RemoveAllConnected();
        }
        currentConnectedSpaces.Clear();
        potentialSpaces.Clear();
        previousConnected = null;
        lastConnected = null;
    }
}