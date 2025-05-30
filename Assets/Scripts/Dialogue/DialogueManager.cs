using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;

    [Header("JSON Input")]
    public TextAsset dialogueJson;

    [Header("Default Dialogue Entries")]
    public DialogueEntry[] defaultDialogues;

    [Header("Speaker Sprites")]
    public SpeakerSpriteData[] speakerSprites;

    [Header("Post Dialogue UI")]
    [SerializeField] private GameObject postDialogueCanvas1;
    [SerializeField] private GameObject postDialogueCanvas2;

    [Tooltip("Speed per character for typewriter effect")]
    public float typingSpeed = 0.05f;

    private DialogueEntry[] dialogues;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;
    private Dictionary<string, Sprite> speakerSpriteMap;
    private Player player;

    [System.Serializable]
    public class SpeakerSpriteData
    {
        public string speakerName;
        public Sprite speakerSprite;
    }

    void Start()
    {
        if (dialogueJson != null)
        {
            try
            {
                DialogueData data = JsonUtility.FromJson<DialogueData>(dialogueJson.text);
                dialogues = (data != null && data.dialogues.Length > 0) ? data.dialogues : defaultDialogues;
            }
            catch
            {
                Debug.LogWarning("DialogueManager: JSON parsing failed. Using default dialogues.");
                dialogues = defaultDialogues;
            }
        }
        else
        {
            dialogues = defaultDialogues;
        }

        speakerSpriteMap = new Dictionary<string, Sprite>();
        foreach (var entry in speakerSprites)
        {
            if (!speakerSpriteMap.ContainsKey(entry.speakerName) && entry.speakerSprite != null)
                speakerSpriteMap.Add(entry.speakerName, entry.speakerSprite);
        }

        SetPanelVisible(false);

        if (postDialogueCanvas1 != null)
            postDialogueCanvas1.SetActive(false);
        if (postDialogueCanvas2 != null)
            postDialogueCanvas2.SetActive(false);
    }

    void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
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

    public void StartDialogue()
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

        DialogueEntry line = dialogues[currentLine];
        dialogueText.text = string.Empty;
        nameText.text = line.name;

        if (speakerSpriteMap.TryGetValue(line.name, out Sprite sprite))
        {
            portraitImage.sprite = sprite;
            portraitImage.enabled = true;
        }
        else
        {
            portraitImage.enabled = false;
        }

        typingCoroutine = StartCoroutine(TypeText(line.line));
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

        if (postDialogueCanvas1 != null)
        {
            postDialogueCanvas1.SetActive(true);
            player.canControl = true;
        }
        if (postDialogueCanvas2 != null)
            postDialogueCanvas2.SetActive(true);
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

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
