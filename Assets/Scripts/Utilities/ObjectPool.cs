using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a pool of objects offscreen. Objects can be requested from the 
/// pool and returned when no longer in use.
/// </summary>
public class ObjectPool {

    Vector3 offscreenPos = new Vector3(1000, 1000, 1000);
    GameObject pooledObject;
    List<GameObject> objectPool;
    int objectPoolSize;

    // Instantiate the objects of our pool
    void FillPool() {
        GameObject newObject;
        for (int i = 0; i < objectPoolSize; i++) {
            newObject = GameObject.Instantiate(pooledObject);
            newObject.AddComponent<PoolObject>();
            newObject.GetComponent<PoolObject>().SetObjectPool(this);
            objectPool.Add(newObject);
            newObject.transform.position = offscreenPos;
        }
    }

    // Creates a new object pool
    public ObjectPool(GameObject prefab, int poolSize) {
        objectPool = new List<GameObject>();
        pooledObject = prefab;
        objectPoolSize = poolSize;
        FillPool();
    }

    // Returns a gameobject to the pool for future use
    public void ReturnToPool(GameObject objectToAdd) {
        objectPool.Add (objectToAdd);
        objectToAdd.transform.position = offscreenPos;
    }

    // Retrieves a gameobject from the pool
    public GameObject GetFromPool() {
        if (objectPool.Count == 0) {
            Debug.LogWarning ("Pool needs to be refilled");
            FillPool ();
        }
        GameObject retrievedObject = objectPool[0];
        objectPool.RemoveAt(0);
        return retrievedObject;
    }

    // Returns all the currently pooled objects
    public List<GameObject> GetPoolObjects() {
        return objectPool;
    }

}
