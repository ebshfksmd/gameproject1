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
                cameraController.SetFollow(false); // 카메라 고정

            if (playerObject != null)
                playerObject.SetActive(false); // 플레이어 비활성화

            if (dialogueManager != null)
                StartCoroutine(StartDialogueThenActivatePlayer());
        }
    }

    private IEnumerator StartDialogueThenActivatePlayer()
    {
        yield return null; // 프레임 대기
        dialogueManager.StartDialogue();

        while (dialogueManager.IsDialogueActive())
        {
            yield return null;
        }

        if (playerObject != null)
            playerObject.SetActive(true); // 플레이어 활성화

        if (cameraController != null)
            cameraController.SetFollow(true); // 카메라 다시 추적
    }
}
