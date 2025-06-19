using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToGame : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadingScreen; // Reference to a loading screen UI object (e.g., Canvas)
    public Slider progressBar;       // Reference to a UI Slider for the loading progress bar
    public Text progressText;        // (Optional) Text to show percentage of loading progress

    // Call this method to start loading the MainGame scene
    public void GoToPlay()
    {
        loadingScreen.SetActive(true);         // Activate the loading screen UI
        StartCoroutine(LoadSceneAsync("MainGame")); // Start loading the scene asynchronously
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent the scene from activating immediately

        // While the scene is still loading, update the loading bar
        while (!operation.isDone)
        {
            // The "progress" value goes from 0 to 0.9, so multiply by 100 to get a percentage
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress; // Update progress bar

            if (progressText != null)
            {
                progressText.text = (progress * 100).ToString("F0") + "%"; // Update progress text if available
            }

            // Check if loading is almost done
            if (operation.progress >= 0.9f)
            {
                // (Optional) Add a delay here for user experience or show "Tap to Continue" UI
                operation.allowSceneActivation = true; // Allow scene activation
            }

            yield return null; // Wait until the next frame to continue
        }
    }
}
