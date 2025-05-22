using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogueEntry
{
    public string name;
    public string line;
}

[System.Serializable]
public class DialogueData
{
    public DialogueEntry[] dialogues;
}

/// <summary>
/// Dialogue window that appears on click and types text inside a rectangle.
/// Loads dialogue entries from a JSON file or uses defaults.
/// Click anywhere to start, advance text, and close.
/// JSON format:
/// {
///   "dialogues": [
///     { "name": "NPC", "line": "Hello." },
///     { "name": "Player", "line": "Hi!" }
///   ]
/// }
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("CanvasGroup for showing/hiding the dialogue panel")]
    public CanvasGroup dialoguePanel;
    [Tooltip("TextMeshPro component for displaying speaker name")]
    public TMP_Text nameText;
    [Tooltip("TextMeshPro component for displaying dialogue text")]
    public TMP_Text dialogueText;

    [Header("JSON Input")]
    [Tooltip("Optional JSON file with dialogue data")]
    public TextAsset dialogueJson;

    [Header("Default Dialogue Entries")]
    [Tooltip("Default dialogue entries if JSON not provided or parsing fails")]
    public DialogueEntry[] defaultDialogues = new DialogueEntry[]
    {
        new DialogueEntry { name = "시스템", line = "안녕하세요! 여기 대화 데모에 오신 것을 환영합니다." },
        new DialogueEntry { name = "강사", line = "이 시스템은 클릭으로 대화를 진행합니다." },
        new DialogueEntry { name = "강사", line = "텍스트가 타자기 효과로 차례대로 나타납니다." },
        new DialogueEntry { name = "시스템", line = "이제 마지막 메시지입니다. 이 대화창이 곧 사라집니다." }
    };

    [Tooltip("Speed per character for typewriter effect")]
    public float typingSpeed = 0.05f;

    private DialogueEntry[] dialogues;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;

    void Start()
    {
        // Load from JSON if available
        if (dialogueJson != null)
        {
            try
            {
                DialogueData data = JsonUtility.FromJson<DialogueData>(dialogueJson.text);
                if (data != null && data.dialogues != null && data.dialogues.Length > 0)
                    dialogues = data.dialogues;
                else
                    dialogues = defaultDialogues;
            }
            catch
            {
                Debug.LogWarning("DialogueManager: JSON parsing failed, using default dialogues.");
                dialogues = defaultDialogues;
            }
        }
        else
        {
            dialogues = defaultDialogues;
        }
        SetPanelVisible(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isDialogueActive)
            {
                StartDialogue();
            }
            else
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    dialogueText.text = dialogues[currentLine].line;
                    typingCoroutine = null;
                }
                else
                {
                    currentLine++;
                    if (dialogues != null && currentLine < dialogues.Length)
                        ShowLine();
                    else
                        EndDialogue();
                }
            }
        }
    }

    private void StartDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            Debug.LogWarning("DialogueManager: No dialogue entries available.");
            return;
        }
        isDialogueActive = true;
        currentLine = 0;
        SetPanelVisible(true);
        ShowLine();
    }

    private void ShowLine()
    {
        if (dialogues == null || currentLine < 0 || currentLine >= dialogues.Length)
        {
            EndDialogue();
            return;
        }
        dialogueText.text = string.Empty;
        if (nameText != null)
            nameText.text = dialogues[currentLine].name;
        typingCoroutine = StartCoroutine(TypeText(dialogues[currentLine].line));
    }

    private IEnumerator TypeText(string line)
    {
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        SetPanelVisible(false);
    }

    private void SetPanelVisible(bool visible)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.alpha = visible ? 1f : 0f;
            dialoguePanel.interactable = visible;
            dialoguePanel.blocksRaycasts = visible;
        }
    }
}
