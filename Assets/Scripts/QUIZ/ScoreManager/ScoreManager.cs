using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int score = 0;
    public TextMeshProUGUI scoreText;  // Current session score display

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScore(); // Load the last saved score at the start of each new quiz
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void SubtractScore(int points)
    {
        score = Mathf.Max(0, score - points);
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    public void UpdateScoreText() // Set this to public
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();
    }

    public void LoadScore()
    {
        score = PlayerPrefs.GetInt("PlayerScore", 0);
        UpdateScoreText();
    }

    public void GoToSavedScoreScene(string sceneName)
    {
        SaveScore(); // Ensures the score is saved before switching scenes
        SceneManager.LoadScene(sceneName);
    }

    public void ResetScore()
{
    score = 0;
    UpdateScoreText(); // Update the displayed score
    SaveScore(); // Optionally save the reset score
}

}
