using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lightweight runtime object pool keyed by prefab — no inspector setup. Spawn with
/// SimplePool.Spawn(prefab, pos, rot) and return with SimplePool.Despawn(go) instead of
/// Instantiate/Destroy, to avoid GC churn from high-frequency projectiles/VFX.
/// Call SimplePool.Reset() on scene load so stale (destroyed) entries don't accumulate.
/// </summary>
public static class SimplePool
{
    private static readonly Dictionary<GameObject, Queue<GameObject>> pools = new Dictionary<GameObject, Queue<GameObject>>();

    public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (prefab == null) return null;

        if (!pools.TryGetValue(prefab, out var q)) { q = new Queue<GameObject>(); pools[prefab] = q; }

        GameObject obj = null;
        while (q.Count > 0) { obj = q.Dequeue(); if (obj != null) break; obj = null; } // skip destroyed

        if (obj == null)
        {
            obj = Object.Instantiate(prefab, pos, rot);
            var marker = obj.AddComponent<PooledInstance>();
            marker.prefab = prefab;
        }
        else
        {
            obj.transform.SetPositionAndRotation(pos, rot);
            obj.SetActive(true); // fires the object's OnEnable (its reset hook)
        }
        return obj;
    }

    public static void Despawn(GameObject obj)
    {
        if (obj == null) return;

        var marker = obj.GetComponent<PooledInstance>();
        if (marker == null || marker.prefab == null) { Object.Destroy(obj); return; } // not pooled

        obj.SetActive(false);
        if (!pools.TryGetValue(marker.prefab, out var q)) { q = new Queue<GameObject>(); pools[marker.prefab] = q; }
        q.Enqueue(obj);
    }

    public static void Reset() => pools.Clear(); // call on scene load
}

/// <summary>Marks a pooled instance with the prefab it came from.</summary>
public class PooledInstance : MonoBehaviour
{
    public GameObject prefab;
}
