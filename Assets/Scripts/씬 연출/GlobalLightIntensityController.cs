using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class GlobalLightIntensityController : MonoBehaviour
{
    [SerializeField] private Slider sliderA;
    [SerializeField] private Slider sliderB;

    [SerializeField] private float minIntensity = 0.1f;
    [SerializeField] private float maxIntensity = 1.0f;

    private Light2D[] allLights;
    private bool isSyncing = false;
    private float currentIntensity = 0.5f; // �����̴� ���ذ� ����

    void Start()
    {
        sliderA.minValue = minIntensity;
        sliderA.maxValue = maxIntensity;
        sliderB.minValue = minIntensity;
        sliderB.maxValue = maxIntensity;

        sliderA.onValueChanged.AddListener(OnAnySliderChanged);
        sliderB.onValueChanged.AddListener(OnAnySliderChanged);

        currentIntensity = sliderA.value;
    }

    void Update()
    {
        allLights = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);

        // �� �����Ӹ��� ����Ʈ ��⸦ �����̴� ���ذ����� ���
        if (allLights != null)
        {
            foreach (var light in allLights)
            {
                if (light != null)
                    light.intensity = currentIntensity;
            }
        }
    }

    public void OnAnySliderChanged(float value)
    {
        if (isSyncing) return;
        isSyncing = true;

        if (sliderA != null && Mathf.Abs(sliderA.value - value) > 0.0001f)
            sliderA.SetValueWithoutNotify(value);
        if (sliderB != null && Mathf.Abs(sliderB.value - value) > 0.0001f)
            sliderB.SetValueWithoutNotify(value);

        currentIntensity = value; // ���ذ� ����

        isSyncing = false;
    }
}
