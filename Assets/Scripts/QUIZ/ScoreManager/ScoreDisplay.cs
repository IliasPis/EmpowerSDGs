using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        int savedScore = PlayerPrefs.GetInt("PlayerScore", 0);  // Load saved score
        scoreText.text = $"Saved Score: {savedScore}";
    }
}
