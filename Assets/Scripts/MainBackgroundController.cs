
using System.Collections.Generic;
using UnityEngine;

public class MainBackgroundController : MonoBehaviour
{
    
    public List<GameObject> flyingEnemies;
    public GameObject flyingPlayer;
    private RectTransform _rectTransform;
    private int _enemiesFlying = 30;

    // Start is called before the first frame update
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        for (var i = 0; i < _enemiesFlying; i++)
        {
            CreateRandomFlyingEnemy();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Remove enemies that are too far away from the screen center
        // Get the background size
        var backgroundRect = _rectTransform.rect;
        var maxDistance = Mathf.Max(backgroundRect.width, backgroundRect.height) * 0.5 + 100;
        
        // Get the enemies that are too far away
        var enemiesToRemove =
            flyingEnemies.FindAll(enemy => enemy.transform.localPosition.magnitude > maxDistance);

        // Remove the enemies
        foreach (var enemy in enemiesToRemove)
        {
            flyingEnemies.Remove(enemy);
            Destroy(enemy);
            CreateRandomFlyingEnemy();
        }
    }

    private void CreateRandomFlyingEnemy()
    {
        // List of enemy prefabs
        var enemyPrefabPaths = new[]
        {
            "Prefabs/WeakEnemy",
            "Prefabs/StrongEnemy",
            "Prefabs/MidEnemy"
        };
        
        // Randomly select an enemy prefab
        var randomEnemyPrefabPath = enemyPrefabPaths[Random.Range(0, enemyPrefabPaths.Length)];
        var enemyPrefab = Resources.Load(randomEnemyPrefabPath);
        var enemy = Instantiate(enemyPrefab, gameObject.transform) as GameObject;

        var randomOffscreenPosition = GetRandomOffscreenPosition();
        var randomVelocity = GetRandomVelocityForPosition(randomOffscreenPosition);
        
        enemy.transform.localPosition = randomOffscreenPosition;
        
        // Make collider a non-trigger, so it will collide with other enemies on the map
        var collider = enemy.GetComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        // Set velocity
        var rigidbody = enemy.GetComponent<Rigidbody2D>();
        rigidbody.velocity = randomVelocity;
        rigidbody.angularVelocity = Random.Range(-50, 50);

        enemy.AddComponent<AutomaticSpriteSwitcher>().switchDelay = 0.4f;

        flyingEnemies.Add(enemy);
    }

    private Vector2 GetRandomVelocityForPosition(Vector2 position)
    {
        // Get the background size
        var backgroundRect = _rectTransform.rect;

        // Select the random position on screen to move to
        var randomPosition = new Vector2(Random.Range(0, backgroundRect.width), Random.Range(0, backgroundRect.height));
        
        // Calculate the velocity to move to the random position
        var velocity = randomPosition - position;
        
        // Normalize the velocity
        velocity.Normalize();
        
        // Scale the velocity by a random speed
        velocity *= Random.Range(100, 300);
        
        return velocity;
    }
    
    private Vector2 GetRandomOffscreenPosition()
    {
        // Get the background size
        var backgroundRect = _rectTransform.rect;
        
        // Generate a random point around the screen
        
        var randomAngle = Random.Range(0, 2 * Mathf.PI);
        var randomVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        
        randomVector *= Mathf.Max(backgroundRect.width, backgroundRect.height) / 2 + 50;

        return randomVector;
    }
}
