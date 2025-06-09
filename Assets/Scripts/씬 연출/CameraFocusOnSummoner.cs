using UnityEngine;

public class CameraFocusOnSummoner : MonoBehaviour
{
    [Header("��� ����")]
    public GameObject targetToActivate;             // �浹 �� Ȱ��ȭ�� ������Ʈ
    public string summonerTag = "Summoner";         // Summoner ������Ʈ�� �±�
    public DialogueManager dialogueManager;         // DialogueManager ����

    [Header("ī�޶� �̵� ����")]
    public float focusDuration = 2f;                // ī�޶� summoner�� �����Ǵ� �ð�

    private Transform originalParent;
    private Vector3 originalPosition;
    private bool isFocusing = false;
    private bool dialogueTriggered = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        originalParent = mainCam.transform.parent;
        originalPosition = mainCam.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFocusing && collision.CompareTag(summonerTag))
        {
            StartCoroutine(FocusOnTarget(collision.transform));
        }
    }

    private System.Collections.IEnumerator FocusOnTarget(Transform target)
    {
        isFocusing = true;

        // ī�޶� ��ġ ����
        mainCam.transform.SetParent(null);
        mainCam.transform.position = new Vector3(target.position.x, target.position.y, originalPosition.z);

        yield return new WaitForSeconds(focusDuration);

        // ������Ʈ Ȱ��ȭ
        if (targetToActivate != null)
            targetToActivate.SetActive(true);

        dialogueTriggered = true;
    }

    void Update()
    {
        if (dialogueTriggered && dialogueManager != null && dialogueManager.IsDialogueFinished)
        {
            // ��� �������� ī�޶� ����
            mainCam.transform.SetParent(originalParent);
            mainCam.transform.position = originalPosition;

            dialogueTriggered = false;
            isFocusing = false;
        }
    }
}
