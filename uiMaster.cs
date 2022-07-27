using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiMaster : MonoBehaviour
{

    [SerializeField] GameObject gameOverTXT;
    [SerializeField] Text score;

    public void gameOverUI()
    {
        gameOverTXT.SetActive(true);
    }

    public void updateScore()
    {
        int temp = int.Parse(score.text);
        temp += 1;
        score.text = temp.ToString();
    }
}
