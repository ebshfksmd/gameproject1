using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraSizeFadeInOnly : MonoBehaviour
{
    public Camera targetCamera;
    public Image fadeImage;
    public float fadeDuration = 2f;
    public float targetCameraSize = 5f;

    public Vector3 fixedCameraPosition = new Vector3(-1, -11f, -1f);
    public float xOffset = 0f;
    public float yOffset = 0f;
    public bool fixCameraPosition = true;

    [Header("���̾�α�")]
    public DialogueManager dialogueManager;  // ���⿡ DialogueManager ����

    void Start()
    {
        if (fadeImage == null || targetCamera == null)
        {
            Debug.LogError("fadeImage �Ǵ� targetCamera�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        SetAlpha(1f); // ���� �� ȭ���� ������ �˰�

        // ���� �� ���̾�α� ��Ȱ��ȭ
        if (dialogueManager != null)
            dialogueManager.gameObject.SetActive(false);

        StartCoroutine(DelayedFadeIn());
    }

    void LateUpdate()
    {
        if (fixCameraPosition && targetCamera != null)
        {
            Vector3 newPos = fixedCameraPosition;
            newPos.x += xOffset;
            newPos.y += yOffset;

            targetCamera.transform.position = newPos;
        }
    }

    IEnumerator DelayedFadeIn()
    {

        // �߰� ��� (1.5�� �� ���̾�α�)
        yield return new WaitForSeconds(1.5f);
        targetCamera.orthographicSize = targetCameraSize;

        if (fixCameraPosition)
            targetCamera.transform.position = fixedCameraPosition;

        float moveDuration = fadeDuration;
        float elapsed = 0f;

        Vector3 startPos = fixedCameraPosition;
        startPos.y = -11f;

        Vector3 endPos = fixedCameraPosition;
        endPos.y = -16.93f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);

            // Lerp position
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
            newPos.x += xOffset;
            newPos.z = fixedCameraPosition.z;

            targetCamera.transform.position = newPos;

            // Simultaneous fade
            float alpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(alpha);

            yield return null;
        }

        // ��ġ ���� �� ���̵� ����
        Vector3 finalPos = endPos;
        finalPos.x += xOffset;
        finalPos.z = fixedCameraPosition.z;
        targetCamera.transform.position = finalPos;
        SetAlpha(0f);


        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(true);
            dialogueManager.StartDialogue();
        }
    }


    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime / 1.2f;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(endAlpha);
    }

    void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
