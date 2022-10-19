using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float minY = -180;
    public float maxY = 180;

    // Update is called once per frame
    private void Update()
    {
        var localPosition = transform.localPosition;
        if (localPosition.y > minY && localPosition.y < maxY) return;
        
        // Create weak explosion
        var weakExplosionPrefab = Resources.Load("Prefabs/WeakExplosion");
        var tr = transform;
        Instantiate(weakExplosionPrefab, tr.position, Quaternion.identity, tr.parent.transform);

        Destroy(gameObject);
    }
}
