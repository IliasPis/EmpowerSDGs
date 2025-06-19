using System.Collections;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    // Singleton instance
    public static Timer Instance { get; private set; }

    // Timer to keep track of playtime
    public static float TimePlayed;

    // UI element to display time if available
    public TextMeshProUGUI TimeDisplay;

    private void Awake()
    {
        // Set up singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Optional: if you want the timer to persist across scenes
    }

    void Start()
    {
        // Optionally, initialize the timer here if needed (e.g., reset at game start)
        // TimePlayed = 0f; // Uncomment to reset the timer at game start
    }

    void Update()
    {
        // Increment the time played
        TimePlayed += Time.deltaTime;

        // Calculate hours, minutes, and seconds
        int hours = Mathf.FloorToInt(TimePlayed / 3600); // 1 hour = 3600 seconds
        int minutes = Mathf.FloorToInt((TimePlayed % 3600) / 60); // Remaining minutes
        int seconds = Mathf.FloorToInt(TimePlayed % 60); // Remaining seconds

        // Format the time as "00 h 00 m 00 sec"
        string formattedTime = string.Format("{0:00} h {1:00} m {2:00} sec", hours, minutes, seconds);

        // Display the formatted time if TimeDisplay is assigned (UI might not always exist)
        if (TimeDisplay != null)
        {
            TimeDisplay.SetText(formattedTime);
        }
    }
}
