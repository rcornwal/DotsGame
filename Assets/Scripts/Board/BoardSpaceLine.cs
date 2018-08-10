using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceLine : MonoBehaviour {

    [Header("References")]
    public GameObject linePrefab;

    const int connectionLinesPerSpace = 2;
    List<ConnectionLine> connectionLines;

	void Start () {
        connectionLines = new List<ConnectionLine>();
        for (int i = 0; i < connectionLinesPerSpace; i++) {
            GameObject newConnectionLine = GameObject.Instantiate(linePrefab);
            ConnectionLine connectionLine = newConnectionLine.GetComponent<ConnectionLine>();
            connectionLines.Add(connectionLine);
        }
	}
	
    public ConnectionLine GetUnusedLine() {
        for (int i = 0; i < connectionLines.Count; i++) {
            if (!connectionLines[i].InUse) {
                return connectionLines[i];
            }
        }
        return null;
    }

    public ConnectionLine GetLast() {
        for (int i = connectionLines.Count - 1; i >= 0; i--) {
            if (connectionLines[i].InUse) {
                return connectionLines[i];
            }
        }
        return null;
    }

    public void SetDotType(DotManager.DotType dotType) {
        for (int i = 0; i < connectionLines.Count; i++) {
            connectionLines[i].SetType(dotType);
        }
    }

    public List<ConnectionLine> GetAllLines() {
        return connectionLines;
    }
}
