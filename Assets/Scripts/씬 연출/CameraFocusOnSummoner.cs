using UnityEngine;

public class CameraFocusOnSummoner : MonoBehaviour
{
    [Header("대상 설정")]
    public GameObject targetToActivate;             // 충돌 후 활성화할 오브젝트
    public string summonerTag = "Summoner";         // Summoner 오브젝트의 태그
    public DialogueManager dialogueManager;         // DialogueManager 참조

    [Header("카메라 이동 설정")]
    public float focusDuration = 2f;                // 카메라가 summoner에 고정되는 시간

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

        // 카메라 위치 고정
        mainCam.transform.SetParent(null);
        mainCam.transform.position = new Vector3(target.position.x, target.position.y, originalPosition.z);

        yield return new WaitForSeconds(focusDuration);

        // 오브젝트 활성화
        if (targetToActivate != null)
            targetToActivate.SetActive(true);

        dialogueTriggered = true;
    }

    void Update()
    {
        if (dialogueTriggered && dialogueManager != null && dialogueManager.IsDialogueFinished)
        {
            // 대사 끝났으면 카메라 복귀
            mainCam.transform.SetParent(originalParent);
            mainCam.transform.position = originalPosition;

            dialogueTriggered = false;
            isFocusing = false;
        }
    }
}
