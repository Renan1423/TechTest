using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public struct Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> Pools;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;
    public Dictionary<string, int> PoolObjectCount;

    private void Start()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();
        PoolObjectCount = new Dictionary<string, int>();

        foreach (Pool pool in Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            PoolDictionary.Add(pool.tag, objectPool);
            PoolObjectCount.Add(pool.tag, 0);
        }
    }

    public GameObject SpawnFromPool(
        string tag,
        Vector3 position,
        Quaternion rotation,
        Transform parent = null
    )
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist!");
            return null;
        }

        GameObject objectToSpawn = DequeueFromPool(tag, parent);

        if (objectToSpawn == null)
            return null;

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.localPosition = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        ObjectFromPool objFromPool = objectToSpawn.GetComponent<ObjectFromPool>();

        objFromPool.OnDisablePooledObject.RemoveAllListeners();
        objFromPool.OnDisablePooledObject.AddListener(
            delegate
            {
                ReEnqueueObject(tag, objectToSpawn);
            }
        );

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }

    public GameObject DequeueFromPool(string tag, Transform parent = null)
    {
        foreach (Pool pool in Pools)
        {
            if (pool.tag != tag)
                continue;

            if (PoolDictionary[tag].Count == 0)
            {
                if (PoolObjectCount[tag] < pool.size)
                {
                    GameObject newObj = Instantiate(pool.prefab, parent);
                    if (parent == null)
                        newObj.transform.SetParent(gameObject.transform);
                    newObj.SetActive(false);

                    PoolObjectCount[tag]++;
                    PoolDictionary[tag].Enqueue(newObj);
                }
                else
                    return null;
            }

            return PoolDictionary[tag].Dequeue();
        }

        return null;
    }

    public void ReEnqueueObject(string tag, GameObject objectToEnqueue)
    {
        PoolDictionary[tag].Enqueue(objectToEnqueue);
    }
}
