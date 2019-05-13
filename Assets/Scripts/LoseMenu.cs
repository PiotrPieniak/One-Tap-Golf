using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseMenu : MonoBehaviour
{
    public Text ScoreDisplay;
    public Text HighscoreText;
    
    void Start()
    {
        HighscoreText.text = PlayerPrefs.GetFloat("Highscore").ToString();
    }

    public void ToggleLoseMenu(float score)
    {
        ScoreDisplay.text = score.ToString();
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }
}
