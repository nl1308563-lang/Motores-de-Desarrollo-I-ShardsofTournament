using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        StartCoroutine(PlayGameDelay());
    }

    private IEnumerator PlayGameDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        SceneManager.LoadSceneAsync(1);
    }

    private IEnumerator QuitGameDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        Application.Quit();
    }
}