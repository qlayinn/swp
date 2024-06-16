using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject highScorePanel;
    public TMP_Text highScoreText;

    private void Start()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("HighScorePanel is not assigned in the inspector.");
        }
    }

    public void PlayGame()
    {
        Debug.Log("Play button clicked");
        SceneManager.LoadScene("APScene");
    }

    public void ShowHighScores()
    {
        if (highScorePanel != null && highScoreText != null)
        {
            highScorePanel.SetActive(true);

            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "High Score: " + highScore + " seconds";
        }
        else
        {
            Debug.LogError("HighScorePanel or HighScoreText is not assigned in the inspector.");
        }
    }

    public void CloseHighScores()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("HighScorePanel is not assigned in the inspector.");
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        SceneManager.LoadScene("ExitConfirmation");
    }
}
