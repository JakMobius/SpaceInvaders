using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    public Sprite[] sprites;
    private int _spriteIndex = 0;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        SwitchSprite();
    }
    
    public void SwitchSprite()
    {
        _image.sprite = sprites[_spriteIndex];
        _spriteIndex = (_spriteIndex + 1) % sprites.Length;
    }
}
