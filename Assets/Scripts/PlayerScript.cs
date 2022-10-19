using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Image _image;
    public float shootDelay = 0.8f;
    public float powerupShootDelay = 0.4f;
    public IBaseGameController GameController;
    public float powerupTime = 7.0f;

    public float powerupTimeLeft = 0.0f;
    
    private float shootTimer = 0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _image = GetComponent<Image>();
        
        _image.color = Color.green;
    }

    private void Update()
    {
        if (powerupTimeLeft > 0)
        {
            powerupTimeLeft -= Time.deltaTime;
            if(powerupTimeLeft <= 0)
            {
                OnPowerupExpire();
            }
        }

        // Move player along the X axis
        var x = Input.GetAxis("Horizontal");
        _rb.velocity = new Vector2(x * 100, _rb.velocity.y);

        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            return;
        }

        // Shoot on shoot axis
        var shoot = Input.GetAxis("Fire1");

        if (shoot <= 0.5) return;

        Shoot();
    }

    private void Shoot()
    {
        var projectilePrefab = Resources.Load("Prefabs/PlayerProjectile");
        
        var tr = transform;
        
        if (powerupTimeLeft <= 0)
        {
            var projectile = Instantiate(projectilePrefab, tr.position, Quaternion.identity, tr.parent.transform);
            projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 400);
            shootTimer = shootDelay;
        }
        else
        {
            for (var i = 0; i < 3; i++)
            {
                var shootAngle = (i - 1) * (Mathf.PI / 9);
                
                var projectile = Instantiate(projectilePrefab, tr.position, Quaternion.identity, tr.parent.transform);
                projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sin(shootAngle) * 400, Mathf.Cos(shootAngle) * 400);
            }
            
            shootTimer = powerupShootDelay;
        }
    }

    private void TriggerPowerup()
    {
        powerupTimeLeft = powerupTime;
        _image.color = Color.red;
    }

    private void OnPowerupExpire()
    {
        _image.color = Color.green;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        var collidedWithEnemy = other.gameObject.CompareTag("Enemy");
        var collidedWithEnemyProjectile = !collidedWithEnemy && other.gameObject.CompareTag("EnemyProjectile");
        var collidedWithPowerUp = !collidedWithEnemy && !collidedWithEnemyProjectile && other.gameObject.CompareTag("Powerup");

        if (collidedWithPowerUp)
        {
            Destroy(other.gameObject);
            TriggerPowerup();
            return;
        }
        
        if (!collidedWithEnemy && !collidedWithEnemyProjectile) return;

        GameController.OnLifeLoss();
            
        var explosionPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
        var explosion = Instantiate(explosionPrefab, GameController.RootObject().transform, false);
        explosion.transform.position = transform.position;

        if (collidedWithEnemyProjectile)
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }
}