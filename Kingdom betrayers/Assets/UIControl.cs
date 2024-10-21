using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public GameObject continueButtonGameOver;

    public GameObject continueButtonWin;
    public GameObject quitButtonGameOver;

    public GameObject quitButtonWin;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    public AudioSource audioSource;
    public AudioClip clip1;

    public AudioClip clip2;

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        audioSource.Stop();
        audioSource.clip = clip1;
        audioSource.Play();
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        audioSource.Stop();
        audioSource.clip = clip2;
        audioSource.Play();
    }

    public void ContinueButtonGameOver()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        SceneManager.LoadScene(currentSceneName);
    }

    public void ContinueButtonWin()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitButtonGameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitButtonWin()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if(MovimientoJugador.vida <= 0)
        {
            ShowGameOverPanel();
        }

        if(AlanController.vida <= 0)
        {
            ShowWinPanel();
        }
    }
}
