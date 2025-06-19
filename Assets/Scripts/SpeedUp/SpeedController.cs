using UnityEngine;
using UnityEngine.UI;

public class SpeedController : MonoBehaviour
{
    public Button speed1xButton;
    public Button speed2xButton;
    public Button speed4xButton;
    public Button resetButton;

    private void Start()
    {
        // Assign methods to button click events
        speed1xButton.onClick.AddListener(SetSpeed1x);
        speed2xButton.onClick.AddListener(SetSpeed2x);
        speed4xButton.onClick.AddListener(SetSpeed4x);
        resetButton.onClick.AddListener(ResetSpeed);

        // Set default game speed to 1x
        SetSpeed1x();
    }

    private void SetSpeed1x()
    {
        Time.timeScale = 1f;
        Debug.Log("Game speed set to 1x");
    }

    private void SetSpeed2x()
    {
        Time.timeScale = 2f;
        Debug.Log("Game speed set to 2x");
    }

    private void SetSpeed4x()
    {
        Time.timeScale = 4f;
        Debug.Log("Game speed set to 4x");
    }

    private void ResetSpeed()
    {
        Time.timeScale = 1f;
        Debug.Log("Game speed reset to normal");
    }

    private void OnDestroy()
    {
        // Reset time scale when the object is destroyed
        Time.timeScale = 1f;
    }
}
