using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // tag -> prefab, so we can recreate an instance if one was destroyed
    // (e.g. an Exploder that Destroy()s itself, or leftovers after a scene reload).
    private Dictionary<string, GameObject> prefabByTag;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        prefabByTag = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary[pool.tag] = objectPool;
            prefabByTag[pool.tag] = pool.prefab;
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        Queue<GameObject> queue = poolDictionary[tag];
        GameObject prefab = prefabByTag.ContainsKey(tag) ? prefabByTag[tag] : null;

        GameObject chosen = null;
        int n = queue.Count;

        // Walk the whole queue once: drop/replace destroyed refs, and prefer an
        // object that is currently inactive so we never teleport a live enemy.
        for (int i = 0; i < n; i++)
        {
            GameObject obj = queue.Dequeue();

            // Unity overloads == so a destroyed object compares equal to null.
            if (obj == null)
            {
                if (prefab == null) continue; // can't recreate; just drop it
                obj = Instantiate(prefab);
                obj.SetActive(false);
            }

            queue.Enqueue(obj);
            if (chosen == null && !obj.activeSelf) chosen = obj;
        }

        // Everything alive (or queue was empty of usable objects): grow the pool.
        if (chosen == null)
        {
            if (prefab == null) return null;
            chosen = Instantiate(prefab);
            chosen.SetActive(false);
            queue.Enqueue(chosen);
        }

        chosen.transform.SetPositionAndRotation(position, rotation);
        chosen.SetActive(true); // fires the enemy's OnEnable (resets HP / colliders)
        return chosen;
    }
}
