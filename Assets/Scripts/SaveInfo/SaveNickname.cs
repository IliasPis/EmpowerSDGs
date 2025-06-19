using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveNickname : MonoBehaviour
{
    // Reference to TMP_InputField for the user to input the nickname
    public TMP_InputField inputNickname;

    // Reference to TextMeshProUGUI for displaying the nickname in the UI (like the one in your screenshot)
    public TextMeshProUGUI displayNickname;

    // Reference to a TextMeshPro component for displaying the nickname (e.g., 3D text in the world)
    public TextMeshProUGUI worldSpaceTextMesh;

    public TextMeshProUGUI worldSpaceTextMeshPause;

    public TextMeshProUGUI worldSpaceTextMeshend;

    void Start()
    {
        // Load the saved nickname when the game starts
        LoadNickname();
    }

    // Method to save the nickname entered by the player
    public void SaveName()
    {
        // Get the nickname from the input field
        string nicknameToSave = inputNickname.text;

        // Save the nickname in PlayerPrefs
        PlayerPrefs.SetString("Nickname", nicknameToSave);
        PlayerPrefs.Save();

        // Update the displayNickname UI text (like the one in your screenshot)
        if (displayNickname != null)
        {
            displayNickname.text = nicknameToSave;
        }

        // Update the worldSpaceTextMesh with the saved nickname if it's assigned
        if (worldSpaceTextMesh != null)
        {
            worldSpaceTextMesh.text = nicknameToSave;
        }

        Debug.Log("Nickname saved as: " + nicknameToSave);
    }

    // Method to load the saved nickname from PlayerPrefs
    public void LoadNickname()
    {
        // Load the nickname from PlayerPrefs (or return an empty string if not found)
        string savedNickname = PlayerPrefs.GetString("Nickname", "");

        // Update the input field with the loaded nickname
        if (inputNickname != null)
        {
            inputNickname.text = savedNickname;
        }

        // Update the displayNickname UI text if it's assigned
        if (displayNickname != null)
        {
            displayNickname.text = savedNickname;
        }

        // Update the worldSpaceTextMesh with the loaded nickname if it's assigned
        if (worldSpaceTextMesh != null)
        {
            worldSpaceTextMesh.text = savedNickname;
            worldSpaceTextMeshPause.text = savedNickname;
            worldSpaceTextMeshend.text = savedNickname;
        }

        Debug.Log("Nickname loaded: " + savedNickname);
    }
}
