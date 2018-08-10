using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    Vector3 offscreenPos = new Vector3(1000, 1000, 1000);
    GameObject pooledObject;
    List<GameObject> objectPool;
    int objectPoolSize;

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

    public ObjectPool(GameObject prefab, int poolSize) {
        objectPool = new List<GameObject>();
        pooledObject = prefab;
        objectPoolSize = poolSize;
        FillPool();
    }

    public ObjectPool(List<GameObject> prefabs, List<int> poolSizes) {
        objectPool = new List<GameObject>();
        for (int i = 0; i < prefabs.Count; i++) {
            pooledObject = prefabs[i];
            objectPoolSize = poolSizes[i];
            FillPool();
        }
    }

    public ObjectPool(List<GameObject> prefabs, int poolSize) {
        objectPool = new List<GameObject>();
        for (int i = 0; i < prefabs.Count; i++) {
            pooledObject = prefabs[i];
            objectPoolSize = poolSize;
            FillPool();
        }
    }

    public void ReturnToPool(GameObject objectToAdd) {
        objectPool.Add (objectToAdd);
        objectToAdd.transform.position = offscreenPos;
    }

    public GameObject GetFromPool() {
        if (objectPool.Count == 0) {
            Debug.LogWarning ("Pool needs to be refilled");
            FillPool ();
        }
        GameObject retrievedObject = objectPool[0];
        objectPool.RemoveAt(0);
        return retrievedObject;
    }

    public void Shuffle() {
        for (int i = 0; i < objectPool.Count; i++) {
            GameObject temp = objectPool[i];
            int randomIndex = Random.Range(i, objectPool.Count);
            objectPool[i] = objectPool[randomIndex];
            objectPool[randomIndex] = temp;
        }
    }

    public GameObject GetRandomFromPool() {
        Shuffle();
        return GetFromPool();
    }

    public int GetPoolSize() {
        return objectPool.Count;
    }

    public List<GameObject> GetPoolObjects() {
        return objectPool;
    }

}
