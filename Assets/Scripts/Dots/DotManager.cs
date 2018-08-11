using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the creation and management of the dot pool. 
/// Assigns each dot their type.
/// </summary>
public class DotManager : MonoBehaviour {

    [System.Serializable]
    public struct DotType {
        public int typeID;
        public Color dotColor;
    }
    public List<DotType> dotTypes;

    [Header("References")]
    public BoardCoordinateSpace boardCoordinateSpace;
    public GameObject dotPrefab;

    ObjectPool dotPool;

	// Use this for initialization
	void Start () {

        // create our dot pool
        int dotPoolSize = boardCoordinateSpace.columns * boardCoordinateSpace.rows * 2;
        dotPool = new ObjectPool(dotPrefab, dotPoolSize);

        // scales the dots to match the screen ratio
        float dotScaleFactor = boardCoordinateSpace.GetDotScaleFactor();
        List<GameObject> dotObjects = dotPool.GetPoolObjects();
        for (int i = 0; i < dotObjects.Count; i++) {
            DotController dot = dotObjects[i].GetComponent<DotController>();
            dot.SetScaleFactor(dotScaleFactor);
        }
    }

    // Retrieves a dot from the pool, with a random type
    public DotController GetNewDot() {
        GameObject dotObject = dotPool.GetFromPool();
        DotController dot = dotObject.GetComponent<DotController>();
        DotType dotType = dotTypes[Random.Range(0, dotTypes.Count)];
        dot.SetDotType(dotType);
        return dot;
    }
	
}
