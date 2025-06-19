using System.Collections;
using UnityEngine;

public class WebGLPerformanceConfig : MonoBehaviour
{
    [Header("Quality Settings")]
    [Tooltip("Set to lower quality settings for better performance.")]
    public int targetQualityLevel = 2; // 0 = Very Low, 1 = Low, 2 = Medium, etc.

    [Header("Texture and Graphics Optimization")]
    [Tooltip("Downscale textures for WebGL for improved performance.")]
    public int maxTextureSize = 1024; // Adjust for higher quality

    [Header("Physics Settings")]
    [Tooltip("Reduce physics calculations by adjusting Fixed Timestep.")]
    public float physicsTimestep = 0.02f; // Default is 0.02f; increasing reduces CPU load.

    private void Awake()
    {
        ApplyQualitySettings();
        OptimizeTextures();
        AdjustPhysicsSettings();

        // Απενεργοποιούμε προσωρινά το async loading για να δούμε αν επηρεάζει τα textures
        // Αν είναι απαραίτητο, ενεργοποίησέ το ξανά μετά τη δοκιμή
        // if (asyncLoading)
        // {
        //     StartCoroutine(LoadAssetsAsync());
        // }
    }

    private void ApplyQualitySettings()
    {
        QualitySettings.SetQualityLevel(targetQualityLevel, true);
        Debug.Log("Quality Level set to: " + QualitySettings.names[targetQualityLevel]);
    }

    private void OptimizeTextures()
    {
        // Ορισμός του `masterTextureLimit` για να μην επηρεαστεί η ποιότητα των textures
        QualitySettings.globalTextureMipmapLimit = 0; // 0 σημαίνει πλήρη ποιότητα για τα textures
        Debug.Log("Texture quality set to maximum (no downscaling)");
    }

    private void AdjustPhysicsSettings()
    {
        // Reduce the frequency of physics updates for lower CPU consumption
        Time.fixedDeltaTime = physicsTimestep;
        Debug.Log("Physics timestep set to: " + physicsTimestep);
    }
}
