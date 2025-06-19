using UnityEngine;
using I2.Loc;

public class LanguageManager : MonoBehaviour
{
    /// <summary>
    /// Changes the language and refreshes the UI.
    /// </summary>
    /// <param name="languageName">The name of the language to switch to.</param>
    public void ChangeLanguage(string languageName)
    {
        // Attempt to change the language
        if (LocalizationManager.HasLanguage(languageName))
        {
            LocalizationManager.CurrentLanguage = languageName;
            Debug.Log($"Language successfully changed to: {languageName}");

            // Force a refresh on all localized components
            LocalizationManager.LocalizeAll(true);
        }
        else
        {
            Debug.LogError($"Language '{languageName}' is not available. Please add it to the I2 Localization system.");
        }
    }
}
