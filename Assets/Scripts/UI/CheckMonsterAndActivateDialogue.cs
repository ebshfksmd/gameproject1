using UnityEngine;

public class CheckForMonster : MonoBehaviour
{
    public GameObject targetParent;         // Monster�� ã�� �θ� ������Ʈ
    public GameObject dialogueObject;       // DialogueManager�� ���� ������Ʈ
    public GameObject objectToDisable;      // �Բ� ��Ȱ��ȭ�� ������Ʈ

    private bool lastState = false;         // ������ ���� ���� (Monster ���� ����)
    private Player player;                  // �÷��̾� ����

    public GameObject object1; // �浹 Ʈ���ſ� ������Ʈ (�繰)
    public GameObject object2; // ��Ȱ��ȭ�� ������Ʈ
    public GameObject object3; // Ȱ��ȭ�� ������Ʈ

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
        // 1. Monster üũ
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

        // 2. ��簡 �������� ������ player.canControl = true
        if (dialogueManager != null && dialogueManager.IsDialogueFinished)
        {
            if (player != null)
                player.canControl = true;
        }

        // 3. S Ű �Է� ó�� (����: ��� �Ϸ� + Ư�� JSON �̸��� ����)
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
