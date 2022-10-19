using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticSpriteSwitcher : MonoBehaviour
{
    public float switchDelay = 0.2f;
    private float switchTimer = 0.0f;
    private SpriteSwitcher _spriteSwitcher;

    // Update is called once per frame
    private void Start()
    {
        _spriteSwitcher = GetComponent<SpriteSwitcher>();
    }

    private void Update()
    {
        switchTimer -= Time.deltaTime;
        if (!(switchTimer <= 0)) return;
        switchTimer += switchDelay;
        _spriteSwitcher.SwitchSprite();
    }
}
