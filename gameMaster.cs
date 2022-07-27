using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameMaster : MonoBehaviour
{
    [SerializeField] objController objectScript;
    [SerializeField] uiMaster ui;
    [SerializeField] GameObject startButton, re_startButton;

    private void Start()
    {
        objectScript.gameStop = true;
    }

    public void StartGame()
    {
        objectScript.gameStop = false;
        startButton.SetActive(false);
    }
    public void Scored()
    {
        ui.updateScore();
    }
    public void Fail()
    {
        ui.gameOverUI();
        re_startButton.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
