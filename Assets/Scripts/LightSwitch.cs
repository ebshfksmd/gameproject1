using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;  // Light2D 사용 시 필요

public class LightSwitch : MonoBehaviour
{
    public Light2D light2D;
    public float fadeDuration = 1f;

    private Coroutine currentCoroutine;

    public void FadeIn()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeLight(0f, 1f));
    }

    public void FadeOut()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeLight(1f, 0f));
    }

    private IEnumerator FadeLight(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            light2D.intensity = Mathf.Lerp(from, to, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        light2D.intensity = to;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            FadeIn();

        if (Input.GetKeyDown(KeyCode.G))
            FadeOut();
    }

}
