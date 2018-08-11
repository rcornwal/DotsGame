using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each board space holds connection lines to visualize the dot path
/// </summary>
public class BoardSpaceLine : MonoBehaviour {

    const int connectionLinesPerSpace = 2;

    [Header("References")]
    public GameObject linePrefab;

    List<ConnectionLine> connectionLines;

	void Start () {
        connectionLines = new List<ConnectionLine>();
        for (int i = 0; i < connectionLinesPerSpace; i++) {
            GameObject newConnectionLine = GameObject.Instantiate(linePrefab);
            ConnectionLine connectionLine = newConnectionLine.GetComponent<ConnectionLine>();
            connectionLines.Add(connectionLine);
        }
	}
	
    // Find a connection line not currently in use
    public ConnectionLine GetUnusedLine() {
        for (int i = 0; i < connectionLines.Count; i++) {
            if (!connectionLines[i].InUse) {
                return connectionLines[i];
            }
        }
        return null;
    }

    // Get the last connection line the space used
    public ConnectionLine GetLast() {
        for (int i = connectionLines.Count - 1; i >= 0; i--) {
            if (connectionLines[i].InUse) {
                return connectionLines[i];
            }
        }
        return null;
    }

    // Matches the color of the connection lines to the give dot type
    public void SetDotType(DotManager.DotType dotType) {
        for (int i = 0; i < connectionLines.Count; i++) {
            connectionLines[i].SetType(dotType);
        }
    }

    // Get all connection lines of the space
    public List<ConnectionLine> GetAllLines() {
        return connectionLines;
    }
}
