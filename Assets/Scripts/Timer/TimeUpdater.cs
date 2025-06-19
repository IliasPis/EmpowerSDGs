using UnityEngine;
using TMPro;

public class TimeUpdater : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay;          // Main display for the time (in this GameObject)
    public TextMeshProUGUI externalTimeDisplay;   // Separate display to update time outside this GameObject

    private static float timePlayed; // Static variable to track the total time played

    private void Awake()
    {
        // Optionally initialize the timer display if this object is active
        if (timeDisplay != null)
        {
            UpdateTimeDisplay();
        }
    }

    private void Update()
    {
        // Increment the time played
        timePlayed += Time.deltaTime;

        // Update the internal display if this GameObject is active
        if (timeDisplay != null && gameObject.activeInHierarchy)
        {
            UpdateTimeDisplay();
        }
        
        // Update the external display independently
        if (externalTimeDisplay != null)
        {
            UpdateExternalTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        timeDisplay.text = FormatTime(timePlayed);
    }

    private void UpdateExternalTimeDisplay()
    {
        externalTimeDisplay.text = FormatTime(timePlayed);
    }

    private string FormatTime(float time)
    {
        // Calculate hours, minutes, and seconds
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Return formatted string as "00 h 00 m 00 s"
        return $"{hours:00} h {minutes:00} m {seconds:00} s";
    }
}
