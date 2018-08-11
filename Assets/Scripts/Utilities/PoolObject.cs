using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Automatically attached to objects that are created through an object pool.
/// Stores a reference to the correct pool
/// </summary>
public class PoolObject : MonoBehaviour {

    public ObjectPool objectPool;

	void Start() {}

	public void SetObjectPool(ObjectPool pool) {
        objectPool = pool;
    }

    public void ReturnToPool() {
        objectPool.ReturnToPool(gameObject);
    }
}
