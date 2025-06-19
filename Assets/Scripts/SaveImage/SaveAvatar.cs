using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; // Required for memory stream
using System; // Required for Convert

public class SaveLoadSprite : MonoBehaviour
{
    // Reference to the Button from which we will save the sprite
    public Button spriteButton;

    // Reference to the Image component where we want to load the saved sprite
    public Image displayImage;

    public Image displayImagePause;

     public Image displayImageEnd;

    void Start()
    {
        // Load the saved sprite when the game starts
        LoadSprite();
    }

    // Method to save the sprite from the button's Image component
    public void SaveSprite()
    {
        if (spriteButton != null)
        {
            // Get the Sprite from the button's Image component
            Sprite spriteToSave = spriteButton.image.sprite;

            // Convert the sprite's texture to a byte array
            Texture2D texture = spriteToSave.texture;
            byte[] textureBytes = texture.EncodeToPNG(); // Convert to PNG format

            // Convert the byte array to a Base64 string
            string base64String = Convert.ToBase64String(textureBytes);

            // Save the Base64 string in PlayerPrefs
            PlayerPrefs.SetString("SavedSpriteData", base64String);
            PlayerPrefs.Save();

            Debug.Log("Sprite saved successfully.");
        }
    }

    // Method to load the saved sprite and assign it to the displayImage component
    public void LoadSprite()
    {
        // Retrieve the Base64 string from PlayerPrefs
        string savedSpriteData = PlayerPrefs.GetString("SavedSpriteData", "");

        if (!string.IsNullOrEmpty(savedSpriteData))
        {
            // Convert the Base64 string back to a byte array
            byte[] textureBytes = Convert.FromBase64String(savedSpriteData);

            // Create a new texture and load the byte array into it
            Texture2D texture = new Texture2D(2, 2); // Initialize with default size
            texture.LoadImage(textureBytes); // Load the texture from the byte array

            // Create a new sprite from the texture
            Sprite loadedSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Assign the loaded sprite to the displayImage component
            if (displayImage != null)
            {
                displayImage.sprite = loadedSprite;
                displayImagePause.sprite = loadedSprite;
                displayImageEnd.sprite = loadedSprite;
            }

            Debug.Log("Sprite loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No sprite data found.");
        }
    }
}
