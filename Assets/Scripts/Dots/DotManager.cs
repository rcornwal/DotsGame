using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float screenScaleFactor = boardCoordinateSpace.GetScaleFactor();
        int dotPoolSize = boardCoordinateSpace.columns * boardCoordinateSpace.rows;
        dotPool = new ObjectPool(dotPrefab, dotPoolSize * 2);
        List<GameObject> dotObjects = dotPool.GetPoolObjects();
        for (int i = 0; i < dotObjects.Count; i++) {
            dotObjects[i].GetComponent<DotController>().SetManager(this);
            dotObjects[i].transform.GetChild(0).localScale = dotObjects[i].transform.GetChild(0).localScale * screenScaleFactor;
        }
    }

    public DotController GetNewDot() {
        GameObject dotObject = dotPool.GetFromPool();
        DotController dot = dotObject.GetComponent<DotController>();
        DotType dotType = dotTypes[Random.Range(0, dotTypes.Count)];
        dot.SetDotType(dotType);
        return dot;
    }
	
}
