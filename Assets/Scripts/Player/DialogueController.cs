using UnityEngine;
using System.Collections;

public class DialogueController : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject playerObject;
    public FollowCamera cameraController;

    private bool hasStarted = false;

    void Start()
    {
        if (!hasStarted)
        {
            hasStarted = true;

            if (cameraController != null)
                cameraController.SetFollow(false); // ī�޶� ����

            if (playerObject != null)
                playerObject.SetActive(false); // �÷��̾� ��Ȱ��ȭ

            if (dialogueManager != null)
                StartCoroutine(StartDialogueThenActivatePlayer());
        }
    }

    private IEnumerator StartDialogueThenActivatePlayer()
    {
        yield return null; // ������ ���
        dialogueManager.StartDialogue();

        while (dialogueManager.IsDialogueActive())
        {
            yield return null;
        }

        if (playerObject != null)
            playerObject.SetActive(true); // �÷��̾� Ȱ��ȭ

        if (cameraController != null)
            cameraController.SetFollow(true); // ī�޶� �ٽ� ����
    }
}
