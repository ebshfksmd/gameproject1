using System;
using UnityEngine;

public class CheckForMonster : MonoBehaviour
{
    public GameObject targetParent;         // Monster�� ã�� �θ� ������Ʈ
    public GameObject dialogueObject;       // DialogueManager�� ���� ������Ʈ
    public GameObject objectToDisable;      // �Բ� ��Ȱ��ȭ�� ������Ʈ

    private bool lastState = false;         // ������ ���� ���� (Monster ���� ����)
    private Player player;                  // �÷��̾� ����

    public GameObject ���; // �浹 Ʈ���ſ� ������Ʈ (�繰)
    public GameObject object2; // ��Ȱ��ȭ�� ������Ʈ
    public GameObject object3; // Ȱ��ȭ�� ������Ʈ
    [SerializeField] private GameObject postDialogueCanvas1;
    [SerializeField] private GameObject postDialogueCanvas2;

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
            if (dialogueManager == null)
            {
                Debug.Log("dialogueManager is null.");
            }
            else if (!dialogueManager.IsDialogueFinished)
            {
                Debug.Log("Dialogue is not finished.");
            }
            else if (dialogueManager.dialogueJson == null)
            {
                Debug.Log("dialogueJson is null.");
            }
            else if (dialogueManager.dialogueJson.name != "Go_2Floor" &&
                     dialogueManager.dialogueJson.name != "Go_3Floor" &&
                     dialogueManager.dialogueJson.name != "Go_5Floor")
            {
                Debug.Log($"dialogueJson name is not one of 'Go_2Floor', 'Go_3Floor', or 'Go_5Floor'. Current name: {dialogueManager.dialogueJson.name}");
            }
            else
            {
                // If all conditions pass, execute the code block
                if (postDialogueCanvas1 != null) postDialogueCanvas1.SetActive(true);
                else Debug.Log("postDialogueCanvas1 is null.");

                if (postDialogueCanvas2 != null) postDialogueCanvas2.SetActive(true);
                else Debug.Log("postDialogueCanvas2 is null.");

                if (object2 != null) object2.SetActive(false);
                else Debug.Log("object2 is null.");

                if (object3 != null) object3.SetActive(true);
                else Debug.Log("object3 is null.");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Trigger] �浹�� ������Ʈ: {other.name} / �±�: {other.tag}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾�� �浹 Ȯ�ε�");
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
