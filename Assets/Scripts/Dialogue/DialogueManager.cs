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

    [Header("Custom Activation")]
    public GameObject objectToDisable;
    public GameObject objectToEnable;

    [Header("Fade Out and Scene Transition")]
    public Image blackFadePanel;
    public GameObject nextObjectToActivate;
    public GameObject thisObjectToDeactivate;

    private DialogueEntry[] dialogues;
    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false;
    private Dictionary<string, Sprite> speakerSpriteMap;
    private Player player;
    private bool isDialogueFinished = false;
    public bool IsDialogueFinished => isDialogueFinished;

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
                string cleanedText = dialogueJson.text.Trim('\uFEFF');
                DialogueData data = JsonUtility.FromJson<DialogueData>(cleanedText);
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
            string key = entry.speakerName.Replace(" ", "");
            if (!speakerSpriteMap.ContainsKey(key) && entry.speakerSprite != null)
                speakerSpriteMap.Add(key, entry.speakerSprite);
        }

        SetPanelVisible(false);
        if (postDialogueCanvas1 != null) postDialogueCanvas1.SetActive(false);
        if (postDialogueCanvas2 != null) postDialogueCanvas2.SetActive(false);

        nameText.text = "";
        dialogueText.text = "";
        dialogueText.ForceMeshUpdate();

        if (blackFadePanel != null)
        {
            Color c = blackFadePanel.color;
            c.a = 0f;
            blackFadePanel.color = c;
            blackFadePanel.gameObject.SetActive(true);
        }
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
        isDialogueFinished = false;
        isDialogueActive = true;
        currentLine = 0;

        nameText.text = "";
        dialogueText.text = "";
        dialogueText.ForceMeshUpdate();

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
        string cleanName = line.name.Trim().Replace(" ", "");
        nameText.text = cleanName;

        dialogueText.text = "";
        dialogueText.ForceMeshUpdate();

        if (speakerSpriteMap.TryGetValue(cleanName, out Sprite sprite))
        {
            portraitImage.sprite = sprite;
            portraitImage.enabled = true;
        }
        else
        {
            portraitImage.enabled = false;
        }

        typingCoroutine = StartCoroutine(TypeText(line.line));

        // 특정 대사 후 오브젝트 토글
        if (cleanName == "이현" && line.line.Trim() == "여기..")
        {
            if (objectToDisable != null) objectToDisable.SetActive(false);
            if (objectToEnable != null) objectToEnable.SetActive(true);
        }
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
            if (player != null)
                player.canControl = true;
        }

        if (postDialogueCanvas2 != null)
            postDialogueCanvas2.SetActive(true);

        nameText.text = "";
        dialogueText.text = "";
        dialogueText.ForceMeshUpdate();

        isDialogueFinished = true;

        if (dialogueJson.name != "1Floor")
        {
            StartCoroutine(FadeAndSwitchObjects());
        }
        //1Floor.json 대사 끝났을 때 특정 오브젝트 활성화
        if (dialogueJson != null && dialogueJson.name == "1Floor")
        {
            if (objectToEnable != null)
                objectToEnable.SetActive(true);
        }
    }


    private IEnumerator FadeAndSwitchObjects()
    {
        if (blackFadePanel != null)
        {
            float duration = 2f;
            float time = 0f;
            Color c = blackFadePanel.color;

            while (time < duration)
            {
                time += Time.deltaTime;
                c.a = Mathf.Clamp01(time / duration);
                blackFadePanel.color = c;
                yield return null;
            }

            c.a = 1f;
            blackFadePanel.color = c;
        }

        if (nextObjectToActivate != null)
            nextObjectToActivate.SetActive(true);

        if (thisObjectToDeactivate != null)
            thisObjectToDeactivate.SetActive(false);
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
