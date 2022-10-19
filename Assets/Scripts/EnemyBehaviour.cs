using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyManager Manager;
    [NonSerialized] public RectTransform RectTransform;
    [NonSerialized] public Rigidbody2D RigidBody;
    [NonSerialized] public SpriteSwitcher SpriteSwitcher;
    public GameObject BulletPrefab; 
    public GameObject DropPrefab; 

    public float killScore = 1.0f;
    
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        RigidBody = GetComponent<Rigidbody2D>();
        SpriteSwitcher = GetComponent<SpriteSwitcher>();
    }

    public void Shoot()
    {
        var tr = transform;
        var bullet = Instantiate(BulletPrefab, tr.position, Quaternion.identity, tr.parent.transform);
        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.down * 300;
    }
    
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Manager.OnEnemyHit(this, otherCollider.transform);
    }

    public void Die()
    {
        if (DropPrefab)
        {
            var tr = transform;
            var drop = Instantiate(DropPrefab, tr.position, Quaternion.identity, tr.parent);
            drop.GetComponent<Rigidbody2D>().velocity = Vector2.down * 100;
        }
        Destroy(gameObject);
    }
}
