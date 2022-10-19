
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Update = UnityEngine.PlayerLoop.Update;

public class LiveManager
{
    public int LivesMax = 3;
    public int Lives = 3;
    
    private readonly List<GameObject> _lifeIcons = new();
    private TextMeshProUGUI _livesText;
    private IBaseGameController _gameController;

    public LiveManager(IBaseGameController gameController)
    {
        _gameController = gameController;
        SetupLifeIcons();
    }

    private void SetupLifeIcons()
    {
        var lifePrefab = Resources.Load("Prefabs/LivesIcon");
        var liveTextPrefab = Resources.Load("Prefabs/LivesText");
        
        for (var i = 0; i < LivesMax; i++)
        {
            var lifeIcon = Object.Instantiate(lifePrefab, _gameController.RootObject().transform) as GameObject;
            var rect = lifeIcon.GetComponent<RectTransform>();
            
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(45 + i * 30, 15);
            
            _lifeIcons.Add(lifeIcon);
        }
        
        _livesText = Object.Instantiate(liveTextPrefab, _gameController.RootObject().transform).GetComponent<TextMeshProUGUI>();
        _livesText.text = Lives.ToString();
        
    }

    private void UpdateAppearance()
    {
        for (var i = 0; i < LivesMax; i++)
        {
            _lifeIcons[i].SetActive(Lives > i);
        }
        _livesText.text = Lives.ToString();
    }

    public bool PopLive()
    {
        if(Lives <= 0) return false;
        Lives--;

        UpdateAppearance();
        return true;
    }

    public void Reset()
    {
        Lives = LivesMax;
        UpdateAppearance();
    }

    public void AddOneLive()
    {
        if (Lives >= LivesMax) return;
        Lives++;
        UpdateAppearance();
    }
}