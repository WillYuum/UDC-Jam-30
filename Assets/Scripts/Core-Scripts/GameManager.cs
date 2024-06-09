using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{

    public void RestartGame()
    {
        Debug.Log("Restarting Game");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}

