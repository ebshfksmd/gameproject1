using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls game brightness using URP Color Adjustments Post Exposure override.
/// Requires a global Volume in the scene with a Color Adjustments override.
/// </summary>
public class URPBrightnessController : MonoBehaviour
{
    [Header("References")]
    public Volume volume;               // Assign the global Volume component
    public Slider brightnessSlider;     // UI Slider (0 = dark, 1 = normal)

    [Header("Exposure Settings")]
    [Tooltip("Minimum post exposure value (darkest)")]
    public float minExposure = -1f;
    [Tooltip("Maximum post exposure value (brightest)")]
    public float maxExposure = 1f;

    private ColorAdjustments colorAdjustments;

    void Awake()
    {
        // Use the newer API instead of the obsolete FindObjectOfType
        if (volume == null)
            volume = Object.FindFirstObjectByType<Volume>();
    }

    void Start()
    {
        if (volume != null && volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            colorAdjustments.active = true;
            if (brightnessSlider != null)
            {
                brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
                OnBrightnessChanged(brightnessSlider.value);
            }
        }
        else
        {
            Debug.LogWarning("Volume profile에 Color Adjustments Override가 없습니다.");
        }
    }

    private void OnBrightnessChanged(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = Mathf.Lerp(minExposure, maxExposure, value);
        }
    }
}
