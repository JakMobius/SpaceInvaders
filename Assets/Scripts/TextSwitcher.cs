
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextSwitcher : MonoBehaviour
{
    private int _index = 0;
    public List<string> texts;
    private TextMeshProUGUI _textMesh;
    public float timeBetweenTexts = 0.5f;
    private float _timeToNextText = 0f;
    // Start is called before the first frame update
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!(_timeToNextText > 0)) return;
        _timeToNextText -= Time.deltaTime;
        if (_timeToNextText < 0)
        {
            SwitchText();
        }
    }

    public void SwitchText()
    {
        _index++;
        if (_index >= texts.Count)
        {
            _index = 0;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        _timeToNextText = timeBetweenTexts;
        _textMesh.text = texts[_index];
    }

    private void OnEnable()
    {
        _index = 0;
        UpdateText();
    }
}
