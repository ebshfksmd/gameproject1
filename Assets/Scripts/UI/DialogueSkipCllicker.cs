using UnityEngine;

public class DialogueSkipClicker : MonoBehaviour
{
    [SerializeField] private DialogueManager[] dialogueManagers;

    private void OnMouseDown()
    {
        foreach (var manager in dialogueManagers)
        {
            if (manager != null && manager.IsDialogueActive())
            {
                manager.EndDialogue();
            }
        }
    }
}
