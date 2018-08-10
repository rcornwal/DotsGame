using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    public ObjectPool objectPool;

	void Start() {
		
	}

	public void SetObjectPool(ObjectPool pool) {
        objectPool = pool;
    }

    public void ReturnToPool() {
        objectPool.ReturnToPool(gameObject);
    }
}
