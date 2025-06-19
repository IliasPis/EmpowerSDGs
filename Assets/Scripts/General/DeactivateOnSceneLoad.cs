using UnityEngine;

public class DeactivateOnSceneLoad : MonoBehaviour
{
    // Name of the GameObject to find and deactivate
    public string objectNameToDeactivate = "CloseThisAfterLoading";

    private void Awake()
    {
        // Find the GameObject by name in the scene
        GameObject objectToDeactivate = GameObject.Find(objectNameToDeactivate);

        // If the object is found, deactivate it
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log($"{objectNameToDeactivate} was found and deactivated.");
        }
        else
        {
            Debug.LogWarning($"Object named '{objectNameToDeactivate}' not found in the scene.");
        }
    }
}
