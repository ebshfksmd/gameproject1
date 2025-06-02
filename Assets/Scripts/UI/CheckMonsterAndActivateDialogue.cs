using UnityEngine;

public class CheckForMonster : MonoBehaviour
{
    public GameObject targetParent;         // Monster를 찾을 부모 오브젝트
    public GameObject dialogueObject;       // DialogueManager가 붙은 오브젝트
    public GameObject objectToDisable;      // 함께 비활성화할 오브젝트

    private bool lastState = false;         // 마지막 상태 저장 (Monster 존재 여부)
    private Player player;                  // 플레이어 참조

    public GameObject object1; // 충돌 트리거용 오브젝트 (사물)
    public GameObject object2; // 비활성화할 오브젝트
    public GameObject object3; // 활성화할 오브젝트

    private bool isPlayerColliding = false;

    private DialogueManager dialogueManager;

    void Start()
    {
        player = FindObjectOfType<Player>();

        if (dialogueObject != null)
        {
            dialogueManager = dialogueObject.GetComponent<DialogueManager>();
        }
    }

    void Update()
    {
        // 1. Monster 체크
        if (targetParent != null && dialogueObject != null && objectToDisable != null)
        {
            bool hasMonster = false;

            foreach (Transform child in targetParent.GetComponentsInChildren<Transform>(true))
            {
                if (child.CompareTag("Monster"))
                {
                    hasMonster = true;
                    break;
                }
            }

            if (hasMonster != lastState)
            {
                if (!hasMonster)
                {
                    dialogueObject.SetActive(true);
                    objectToDisable.SetActive(false);
                    if (player != null) player.canControl = false;
                }
                else
                {
                    dialogueObject.SetActive(false);
                    objectToDisable.SetActive(true);
                    if (player != null) player.canControl = true;
                }

                lastState = hasMonster;
            }
        }

        // 2. 대사가 끝났으면 무조건 player.canControl = true
        if (dialogueManager != null && dialogueManager.IsDialogueFinished)
        {
            if (player != null)
                player.canControl = true;
        }

        // 3. S 키 입력 처리 (조건: 대사 완료 + 특정 JSON 이름일 때만)
        if (isPlayerColliding && Input.GetKeyDown(KeyCode.S))
        {
            if (dialogueManager != null &&
                dialogueManager.IsDialogueFinished &&
                dialogueManager.dialogueJson != null &&
                dialogueManager.dialogueJson.name == "1Floor_end")
            {
                if (object2 != null) object2.SetActive(false);
                if (object3 != null) object3.SetActive(true);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerColliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerColliding = false;
        }
    }
}
