using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager
{
    private enum EnemyMoveDirection
    {
        Left,
        Bottom,
        Right
    }

    private readonly List<GameObject> _enemyGrid = new();
    private readonly IBaseGameController _gameController;
    private GameObject _ufo;
    private EnemyMoveDirection _enemyMoveDirection = EnemyMoveDirection.Left;
    
    public float EnemyMoveDelay = 0.033f;
    public float EnemyShootDelay = 1.0f;
    public float UfoDelay = 20.0f;
    
    private float _ufoTimer = 20.0f;

    private float _enemyMoveTimer;
    private float _enemyShootTimer;
    private int _currentEnemyIndex = -1;
    private int _gridCount;
    private bool _paused;
    
    private float _gridMinX = float.PositiveInfinity;
    private float _gridMaxX = float.NegativeInfinity;

    private const int GridRows = 5;
    private const int GridColumns = 11;

    public EnemyManager(IBaseGameController gameController)
    {
        _gameController = gameController;
    }

    private void ClearGrid()
    {
        foreach (var entity in _enemyGrid.Where(entity => entity != null))
        {
            Object.Destroy(entity);
        }
        _enemyGrid.Clear();
        _gridCount = 0;
    }

    public void GenerateEnemies()
    {
        ClearGrid();
        
        var strongEnemy = Resources.Load("Prefabs/StrongEnemy") as GameObject;
        var midEnemy = Resources.Load("Prefabs/MidEnemy") as GameObject;
        var weakEnemy = Resources.Load("Prefabs/WeakEnemy") as GameObject;
        
        const int width = 360;

        GameObject[] enemyRows =
        {
            weakEnemy, weakEnemy, midEnemy, midEnemy, strongEnemy
        };

        var gridVerticalPosition = 50;

        var gridWidth = width * 0.8;
        var gridHeight = width * 0.4;

        var gridOffsetTop = -gridHeight * 0.5 + gridVerticalPosition;
        var gridOffsetLeft = -gridWidth * 0.5;

        var gridMarginLeft = gridWidth / (GridColumns - 1);
        var gridMarginTop = gridHeight / (GridRows - 1);

        for (var y = 0; y < GridRows; y++)
        {
            for (var x = 0; x < GridColumns; x++)
            {
                // Instantiate weakEnemy on the current gameObject
                var enemy = Object.Instantiate(enemyRows[y], _gameController.RootObject().transform, false);

                // Set the position of the enemy
                enemy.transform.localPosition = new Vector3(
                    (float)(gridOffsetLeft + x * gridMarginLeft),
                    (float)(gridOffsetTop + y * gridMarginTop),
                    0
                );

                enemy.GetComponent<EnemyBehaviour>().Manager = this;

                // Add enemy to enemies list
                _enemyGrid.Add(enemy);
                _gridCount++;
            }
        }
    }

    private void DetermineNextEnemyMoveDirection()
    {
        const int rightBoundary = 144;
        const int leftBoundary = -rightBoundary;

        switch (_enemyMoveDirection)
        {
            case EnemyMoveDirection.Left:
                if (_gridMinX < leftBoundary)
                {
                    _enemyMoveDirection = EnemyMoveDirection.Bottom;
                }

                break;
            case EnemyMoveDirection.Right:
                if (_gridMaxX > rightBoundary)
                {
                    _enemyMoveDirection = EnemyMoveDirection.Bottom;
                }

                break;
            case EnemyMoveDirection.Bottom:
                _enemyMoveDirection = _gridMinX + _gridMaxX < 0 ? EnemyMoveDirection.Right : EnemyMoveDirection.Left;
                break;
            default:
                _enemyMoveDirection = EnemyMoveDirection.Bottom;
                break;
        }
    }

    private GameObject GetNextEnemy()
    {
        GameObject enemy;

        var looped = false;
        do
        {
            _currentEnemyIndex++;

            if (_currentEnemyIndex >= _enemyGrid.Count)
            {
                if (looped)
                {
                    return null;
                }
                looped = true;
                DetermineNextEnemyMoveDirection();

                _gridMinX = float.PositiveInfinity;
                _gridMaxX = float.NegativeInfinity;
                _currentEnemyIndex = 0;
            }

            enemy = _enemyGrid[_currentEnemyIndex];
        } while (!enemy);

        return enemy;
    }

    private void MoveEnemies()
    {
        var enemy = GetNextEnemy();
        if (!enemy) return;
        
        var enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        var enemyPosition = enemyBehaviour.RectTransform.localPosition;

        const float enemyMoveSpeed = 6f;

        var enemyMoveDirection = _enemyMoveDirection switch
        {
            EnemyMoveDirection.Left => Vector2.left,
            EnemyMoveDirection.Bottom => Vector2.down,
            EnemyMoveDirection.Right => Vector2.right,
            _ => Vector2.zero
        };

        _gridMinX = Mathf.Min(_gridMinX, enemyPosition.x);
        _gridMaxX = Mathf.Max(_gridMaxX, enemyPosition.x);

        enemyBehaviour.RigidBody.position += enemyMoveDirection * enemyMoveSpeed;
        enemyBehaviour.SpriteSwitcher.SwitchSprite();
    }

    private void ShootRandomEnemy()
    {
        // Get list of not-empty columns
        var notEmptyColumns = new List<int>();
        for (var x = 0; x < GridColumns; x++)
        {
            for (var y = 0; y < GridRows; y++)
            {
                if (!_enemyGrid[y * GridColumns + x]) continue;
                notEmptyColumns.Add(x);
                break;
            }
        }

        if (notEmptyColumns.Count == 0) return;
        
        // Get random column
        var randomColumn = notEmptyColumns[Random.Range(0, notEmptyColumns.Count)];
        
        // Get bottom enemy in the column
        GameObject enemy = null;

        for (var y = 0; y < GridRows; y++)
        {
            enemy = _enemyGrid[y * GridColumns + randomColumn];
            if (enemy) break;
        }
        
        // Shoot
        if (enemy)
        {
            enemy.GetComponent<EnemyBehaviour>().Shoot();
        }
    }
    
    private void SpawnUFO()
    {
        var ufo = Resources.Load("Prefabs/EnemyUFO") as GameObject;
        var ufoInstance = Object.Instantiate(ufo, _gameController.RootObject().transform, false);
        ufoInstance.GetComponent<EnemyBehaviour>().Manager = this;
        
        _ufo = ufoInstance;
        _ufo.transform.localPosition = new Vector2(200, 140);
        
        _ufo.GetComponent<Rigidbody2D>().velocity = new Vector2(-100, 0);
    }
    
    public void Update()
    {
        if (_paused) return;

        _enemyMoveTimer -= Time.deltaTime;
        if (_enemyMoveTimer <= 0)
        {
            _enemyMoveTimer += EnemyMoveDelay;
            MoveEnemies();
        }
        
        _enemyShootTimer -= Time.deltaTime;
        if (_enemyShootTimer <= 0)
        {
            _enemyShootTimer += EnemyShootDelay;
            ShootRandomEnemy();
        }

        _ufoTimer -= Time.deltaTime;
        if (_ufoTimer <= 0)
        {
            _ufoTimer += UfoDelay;
            SpawnUFO();
        }
        else
        {
            UpdateUFO();
        }
    }

    private void UpdateUFO()
    {
        if (!_ufo)
        {
            return;
        }

        if (!(Mathf.Abs(_ufo.transform.localPosition.x) > 200)) return;
        
        Object.Destroy(_ufo);
        _ufo = null;
    }

    public void OnEnemyHit(EnemyBehaviour enemy, Transform otherTransform)
    {
        // Find out if the other object has tag "PlayerProjectile"
        // If it does, destroy the enemy and the projectile

        var enemyTransform = enemy.transform;

        if (!otherTransform.CompareTag("PlayerProjectile")) return;
        
        // Remove enemy from the list and reduce currentIndex if it's necessary
        var enemyIndex = _enemyGrid.IndexOf(enemyTransform.gameObject);
        if (enemyIndex != -1)
        {
            _enemyGrid[enemyIndex] = null;
            _gridCount--;
        }

        // Instantiate an explosion
        var explosionPrefab = Resources.Load("Prefabs/Explosion") as GameObject;
        var explosion = Object.Instantiate(explosionPrefab, _gameController.RootObject().transform, false);
        explosion.transform.position = enemyTransform.position;

        _gameController.AddScore(enemyTransform.GetComponent<EnemyBehaviour>().killScore);

        enemy.Die();
        Object.Destroy(otherTransform.gameObject);

        if (_gridCount == 0 && !_paused)
        {
            _gameController.OnWin();
        }
    }

    public void Resume()
    {
        _paused = false;
    }

    public void Pause()
    {
        _paused = true;
    }
}