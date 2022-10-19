using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour, IBaseGameController
{
    private enum GameState
    {
        Playing,
        Death,
        Win,
        GameOver
    }
    
    // Start is called before the first frame update

    public TextMeshProUGUI scoreLabel;
    public TextMeshProUGUI highScoreLabel;
    public TextMeshProUGUI gameOverLabel;

    private GameObject _player;

    private float _score;
    private float _highScore;
    private EnemyManager _enemyManager;
    private LiveManager _liveManager;
    private GameState _gameState;

    public float restartDelay = 1.0f;
    private float _restartTimer = 0;

    private void Awake()
    {
        _enemyManager = new EnemyManager(this);
        _liveManager = new LiveManager(this);

        gameOverLabel.gameObject.SetActive(false);
        _highScore = PlayerPrefs.GetFloat("highScore");

        UpdateHighScoreLabel();

        NextWave();
    }

    private void UpdateScoreLabel()
    {
        scoreLabel.text = Mathf.RoundToInt(_score).ToString("0000");
    }

    private void UpdateHighScoreLabel()
    {
        highScoreLabel.text = Mathf.RoundToInt(_highScore).ToString("0000");
    }

    private void UpdateHighScore()
    {
        if (!(_score > _highScore)) return;
        _highScore = Mathf.RoundToInt(_score);
        PlayerPrefs.SetFloat("highScore", _highScore);
        UpdateScoreLabel();
    }

    // Update is called once per frame
    private void Update()
    {
        _enemyManager.Update();

        if (_restartTimer < 0) return;
        _restartTimer -= Time.deltaTime;

        if (_restartTimer > 0) return;
        OnTimerFired();
    }

    public void AddScore(float score)
    {
        _score += score;
        UpdateScoreLabel();
    }

    private void RespawnPlayer()
    {
        if (!_player)
        {
            _player = Instantiate(Resources.Load("Prefabs/Player"), transform) as GameObject;
            _player.GetComponent<PlayerScript>().GameController = this;
        }

        _player.transform.localPosition = new Vector2(0, -160);
    }

    private void GameOver()
    {
        UpdateHighScore();
        _gameState = GameState.GameOver;
        gameOverLabel.gameObject.SetActive(true);
    }

    private void UseNextLife()
    {
        _gameState = GameState.Playing;
        _enemyManager.Resume();
        RespawnPlayer();
    }

    private void NextWave()
    {
        UpdateScoreLabel();
        _gameState = GameState.Playing;
        _enemyManager.GenerateEnemies();
        RespawnPlayer();
        _enemyManager.Resume();
    }

    private void OnTimerFired()
    {
        switch (_gameState)
        {
            case GameState.Death:
                if (_liveManager.PopLive()) {
                    UseNextLife();
                } else {
                    GameOver();
                }

                break;
            case GameState.Win:
                _liveManager.AddOneLive();
                NextWave();
                break;
            default: break;
        }
    }

    public void OnWin()
    {
        _gameState = GameState.Win;
        _enemyManager.Pause();
        _restartTimer = restartDelay;
    }

    public void OnLifeLoss()
    {
        _gameState = GameState.Death;
        _player = null;
        _enemyManager.Pause();
        _restartTimer = restartDelay;
    }

    public GameObject RootObject()
    {
        return gameObject;
    }
}