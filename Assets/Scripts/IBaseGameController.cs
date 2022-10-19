
using UnityEngine;

public interface IBaseGameController
{
    public void OnWin();
    public void OnLifeLoss();

    public void AddScore(float score);

    public GameObject RootObject();
}