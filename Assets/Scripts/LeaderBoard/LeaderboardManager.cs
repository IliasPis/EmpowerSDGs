using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    // Arrays for each data type, holding 7 slots
    public TMP_Text[] playerNames;
    public Image[] playerAvatars;
    public TMP_Text[] timePlayedTexts;
    public TMP_Text[] scoreTexts;
    public TMP_Text[] rankNumbers;

    private List<PlayerData> leaderboard = new List<PlayerData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Activate leaderboard UI only in MainMenu
        gameObject.SetActive(scene.name == "MainMenu");
    }

    void Start()
    {
        LoadLeaderboard();
        DisplayLeaderboard();
    }

    public void AddNewScore(string playerName, Sprite playerAvatar, float timePlayed, int score)
    {
        PlayerData newEntry = new PlayerData
        {
            PlayerName = playerName,
            Avatar = playerAvatar,
            TimePlayed = timePlayed,
            Score = score
        };

        leaderboard.Add(newEntry);
        leaderboard = leaderboard.OrderByDescending(p => p.Score).ThenBy(p => p.TimePlayed).Take(7).ToList();

        SaveLeaderboard();
        DisplayLeaderboard();
    }

    private void LoadLeaderboard()
    {
        leaderboard.Clear();

        for (int i = 0; i < 7; i++)
        {
            if (PlayerPrefs.HasKey($"PlayerName_{i}"))
            {
                string name = PlayerPrefs.GetString($"PlayerName_{i}");
                float time = PlayerPrefs.GetFloat($"TimePlayed_{i}");
                int score = PlayerPrefs.GetInt($"Score_{i}");

                string avatarBase64 = PlayerPrefs.GetString($"Avatar_{i}", "");
                Sprite avatar = null;
                
                if (!string.IsNullOrEmpty(avatarBase64))
                {
                    Texture2D avatarTexture = new Texture2D(2, 2);
                    avatarTexture.LoadImage(System.Convert.FromBase64String(avatarBase64));
                    avatar = Sprite.Create(avatarTexture, new Rect(0, 0, avatarTexture.width, avatarTexture.height), new Vector2(0.5f, 0.5f));
                }

                leaderboard.Add(new PlayerData { PlayerName = name, Avatar = avatar, TimePlayed = time, Score = score });
            }
        }

        leaderboard = leaderboard.OrderByDescending(p => p.Score).ThenBy(p => p.TimePlayed).Take(7).ToList();
    }

    private void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerPrefs.SetString($"PlayerName_{i}", leaderboard[i].PlayerName);
            PlayerPrefs.SetFloat($"TimePlayed_{i}", leaderboard[i].TimePlayed);
            PlayerPrefs.SetInt($"Score_{i}", leaderboard[i].Score);

            if (leaderboard[i].Avatar != null && leaderboard[i].Avatar.texture != null)
            {
                byte[] avatarBytes = leaderboard[i].Avatar.texture.EncodeToPNG();
                string avatarBase64 = System.Convert.ToBase64String(avatarBytes);
                PlayerPrefs.SetString($"Avatar_{i}", avatarBase64);
            }
        }

        PlayerPrefs.Save();
    }

    private void DisplayLeaderboard()
    {
        for (int i = 0; i < 7; i++)
        {
            if (i < leaderboard.Count)
            {
                playerNames[i].text = leaderboard[i].PlayerName;
                playerAvatars[i].sprite = leaderboard[i].Avatar;

                int hours = Mathf.FloorToInt(leaderboard[i].TimePlayed / 3600);
                int minutes = Mathf.FloorToInt((leaderboard[i].TimePlayed % 3600) / 60);
                int seconds = Mathf.FloorToInt(leaderboard[i].TimePlayed % 60);
                timePlayedTexts[i].text = $"{hours:00}h {minutes:00}m {seconds:00}s";

                scoreTexts[i].text = leaderboard[i].Score.ToString();
                rankNumbers[i].text = (i + 1).ToString();
            }
            else
            {
                // Clear unused slots
                playerNames[i].text = "";
                playerAvatars[i].sprite = null;
                timePlayedTexts[i].text = "";
                scoreTexts[i].text = "";
                rankNumbers[i].text = (i + 1).ToString();
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string PlayerName;
        public Sprite Avatar;
        public float TimePlayed;
        public int Score;
    }
}
