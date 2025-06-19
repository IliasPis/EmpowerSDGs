using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDataSaver : MonoBehaviour
{
    public TextMeshProUGUI playerNameText; // Το όνομα του παίκτη
    public Image playerAvatar; // Το avatar του παίκτη
    public TextMeshProUGUI timePlayedText; // Χρόνος παιχνιδιού
    public TextMeshProUGUI scoreText; // Σκορ του παίκτη

    private void SavePlayerData()
    {
        // Check for null references and log if any are missing
        if (playerNameText == null || playerAvatar == null || timePlayedText == null || scoreText == null)
        {
            Debug.LogError("One or more UI elements are not assigned in the inspector.");
            return;
        }

        // Retrieve data and check if external references are available
        string playerName = playerNameText.text;
        Sprite avatar = playerAvatar.sprite;

        // Access TimePlayed as a static field without an instance
        float timePlayed = Timer.TimePlayed;

        // Ensure ScoreManager instance is available before getting the score
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance is not found.");
            return;
        }

        int score = ScoreManager.Instance.GetScore();

        // Ensure LeaderboardManager is available before saving
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.AddNewScore(playerName, avatar, timePlayed, score);
            Debug.Log("Τα δεδομένα του παίκτη αποθηκεύτηκαν για το leaderboard.");
        }
        else
        {
            Debug.LogError("LeaderboardManager instance is not found.");
        }
    }

    // Method to save player data
    public void SaveToLeaderboard()
    {
        SavePlayerData();
    }
}
