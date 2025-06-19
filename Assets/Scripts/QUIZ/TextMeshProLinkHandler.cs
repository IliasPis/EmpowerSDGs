using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class TextMeshProLinkHandler : MonoBehaviour, IPointerClickHandler
{
    [DllImport("__Internal")]
    private static extern void OpenURL(string url);

    private TextMeshProUGUI textMeshPro;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if a link was clicked in the TextMeshPro component
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, null);

        if (linkIndex != -1) // If a link was clicked
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();

            #if UNITY_WEBGL && !UNITY_EDITOR
                OpenURL(linkId);  // Call the JavaScript function to open the link
            #else
                Application.OpenURL(linkId);  // Open URL directly in the editor or other platforms
            #endif

            Debug.Log("Clicked link with URL: " + linkId);
        }
    }
}
