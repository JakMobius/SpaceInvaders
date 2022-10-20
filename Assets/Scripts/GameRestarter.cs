using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRestarter : MonoBehaviour
{
    // When "R" is pressed, reload the scene
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}
