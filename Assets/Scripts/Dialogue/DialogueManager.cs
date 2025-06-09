using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [Header("Custom Activation")]
    public GameObject objectToDisable;
    public GameObject objectToEnable;
    public GameObject objectToEnable2;

    [Header("Fade Out and Scene Transition")]
    public Image blackFadePanel;
    public GameObject nextObjectToActivate;
    public GameObject thisObjectToDeactivate;

    [Header("Dialogue Skip Object")]
    [SerializeField] private GameObject skipTriggerObject;

    public float typingSpeed = 0.05f;

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

        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if (!isDialogueActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipCurrentDialogue();
        }
    }

    public void StartDialogue()
    {
        if (skipTriggerObject != null) skipTriggerObject.SetActive(true);

        isDialogueFinished = false;
        isDialogueActive = true;
        currentLine = 0;

        if (player != null)
        {
            player.canControl = false;
        }

        PlayerSkillController.canAttack = false;
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

        if (cleanName == "����" && line.line.Trim() == "��..��?")
        {
            if (objectToDisable != null) objectToDisable.SetActive(false);
            if (objectToEnable != null) objectToEnable.SetActive(true);
        }
        if (cleanName == "�Ƴ��" && line.line.Trim() == "������ ������ �����ڴ� ���� ��� ���б������ �ٹ��ߴ� ��������� ä�� ����, ������ �׸��� 2�� ������ ������ ��ȹ�� ������ �˷������ϴ�.")
        {
            if (objectToDisable != null) objectToDisable.SetActive(true);
        }
        if (cleanName == "�Ƴ��" && line.line.Trim() == "ä ���� ������ �����ϰ��� �ϴ� ������� ������ ����������, �� �������� �η� ���ݼ��� ���� �����ڸ� ������ٰ� �����߽��ϴ�.")
        {
            if (objectToEnable != null) objectToEnable.SetActive(true);
            if (objectToDisable != null) objectToDisable.SetActive(false);
            if (objectToEnable2 != null) objectToEnable2.SetActive(true);
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

    public void SkipCurrentDialogue()
    {
        if (!isDialogueActive || dialogues == null || currentLine >= dialogues.Length)
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogues[currentLine].line;
            typingCoroutine = null;
        }
        else
        {
            currentLine++;
            if (currentLine < dialogues.Length)
            {
                ShowLine();
            }
            else
            {
                EndDialogue();
            }
        }
    }

    public void EndDialogue()
    {
        Debug.Log("EndDialogue: ��� ���� ó�� ����");

        isDialogueActive = false;
        PlayerSkillController.canAttack = true;
        SetPanelVisible(false);

        if (objectToDisable != null) objectToDisable.SetActive(false);
        if(skipTriggerObject!=null) skipTriggerObject.SetActive(false);
        if (postDialogueCanvas1 != null) postDialogueCanvas1.SetActive(true);
        if (postDialogueCanvas2 != null) postDialogueCanvas2.SetActive(true);

        if (player != null)
        {
            player.canControl = true;
            Debug.Log($"[EndDialogue] player.canControl = {player.canControl}");
        }
        else
        {
            Debug.LogWarning("[EndDialogue] player is NULL");
        }

        Debug.Log($"[EndDialogue] PlayerSkillController.canAttack = {PlayerSkillController.canAttack}");

        nameText.text = "";
        dialogueText.text = "";
        dialogueText.ForceMeshUpdate();

        isDialogueFinished = true;
        if (dialogueJson != null && dialogueJson.name == "5_Floor_end")
        {
            Debug.Log("[EndDialogue] '5_Floor_end' ���� �� ������Ʈ ��ȯ ó��");

            if (nextObjectToActivate != null)
                nextObjectToActivate.SetActive(true);

            if (thisObjectToDeactivate != null)
                thisObjectToDeactivate.SetActive(false);

            if(objectToDisable!= null) objectToDisable.SetActive(false);
        }
        if (dialogueJson != null && dialogueJson.name == "1Floor")
        {
            if (objectToEnable != null)
                objectToEnable.SetActive(true);
        }
        else
        {
            StartCoroutine(FadeAndSwitchObjects());
        }
        if (dialogueJson != null && dialogueJson.name == "Ending")
        {
            Debug.Log("[EndDialogue] Ending ��� ���� �� �� �ʱ�ȭ");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
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
