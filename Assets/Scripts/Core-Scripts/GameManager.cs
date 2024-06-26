using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.GenericSingletons;

public class GameManager : MonoBehaviourSingleton<GameManager>
{



    public void Start()
    {
        StartCoroutine(CheckIfBanksLoaded(() =>
        {
            GameloopManager gameloopManager = FindObjectOfType<GameloopManager>();
            gameloopManager.StartGame();
        }));
    }

    private IEnumerator CheckIfBanksLoaded(System.Action callback)
    {
        string bankName = "Master";
        yield return new WaitUntil(() => FMODUnity.RuntimeManager.HasBankLoaded(bankName));
        Debug.Log("|GameManager|: Bank loaded: " + bankName);

        //add minor delay:
        yield return new WaitForSeconds(2f);

        callback.Invoke();
    }

    public void RestartGame(bool skipIntro = false)
    {
        Debug.Log("Restarting Game");
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        StartCoroutine(CheckIfBanksLoaded(() =>
        {
            GameloopManager gameloopManager = FindObjectOfType<GameloopManager>();

            if (skipIntro)
            {
                gameloopManager.StartGameLoop();
            }
            else
            {
                gameloopManager.StartGame();
            }

        }));
    }
}

