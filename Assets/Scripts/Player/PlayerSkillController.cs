using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Assign Skills")]
    public SkillSO attack;
    public SkillSO qSkill;
    public SkillSO wSkill;
    public SkillSO eSkill;
    public SkillSO rSkill;

    [Header("Audio")]
    public AudioClip F, Q, W, E, R;

    [Header("Cooldown UI")]
    public Image[] skillCooldownFills = new Image[4];
    public TextMeshProUGUI[] skillCooldownTexts = new TextMeshProUGUI[4];

    private Dictionary<SkillSO, float> cooldownTimers = new Dictionary<SkillSO, float>();
    private Animator anim;
    private Player player;
    private AudioSource skillAudioSource;
    private DialogueManager dialogueManager;

    public static bool IsCastingSkill { get; private set; } = false;
    [HideInInspector] public static bool canAttack = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        skillAudioSource = gameObject.AddComponent<AudioSource>();

        foreach (var skill in new[] { qSkill, wSkill, eSkill, rSkill })
        {
            if (skill != null)
                cooldownTimers[skill] = 0f;
        }

        foreach (var fill in skillCooldownFills)
        {
            if (fill != null) fill.gameObject.SetActive(false);
        }

        foreach (var text in skillCooldownTexts)
        {
            if (text != null) text.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();

        if (!canAttack || (dialogueManager != null && dialogueManager.IsDialogueActive()))
            return;

        var skillKeys = new List<SkillSO>(cooldownTimers.Keys);
        foreach (var skill in skillKeys)
        {
            if (skill == null || !cooldownTimers.ContainsKey(skill)) continue;

            int i = GetSkillIndex(skill);
            cooldownTimers[skill] -= Time.deltaTime;
            float t = Mathf.Max(0, cooldownTimers[skill]);

            if (i >= 0)
            {
                if (skillCooldownFills[i] != null)
                {
                    skillCooldownFills[i].gameObject.SetActive(t > 0);
                    skillCooldownFills[i].fillAmount = t / skill.cooldown;
                }

                if (skillCooldownTexts[i] != null)
                {
                    skillCooldownTexts[i].gameObject.SetActive(t > 0);
                    skillCooldownTexts[i].text = $"{t:F1}";
                }
            }
        }

        TryCast(KeyCode.F, attack);
        TryCast(KeyCode.Q, qSkill);
        TryCast(KeyCode.W, wSkill);
        TryCast(KeyCode.E, eSkill);
        TryCast(KeyCode.R, rSkill);
    }

    private void TryCast(KeyCode key, SkillSO skill)
    {
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();

        bool isDialogue = dialogueManager != null && dialogueManager.IsDialogueActive();
        if (skill == null || !canAttack || IsCastingSkill || isDialogue) return;

        if (!cooldownTimers.ContainsKey(skill))
            cooldownTimers[skill] = 0f;

        if (Input.GetKeyDown(key) && cooldownTimers[skill] <= 0f)
            StartCoroutine(CastRoutine(skill, key));
    }

    private IEnumerator CastRoutine(SkillSO skill, KeyCode keyUsed)
    {
        if (player != null) player.canControl = false;
        IsCastingSkill = true;

        if (!string.IsNullOrEmpty(skill.animationName))
            anim?.Play(skill.animationName);

        skillAudioSource.PlayOneShot(GetClipByKey(keyUsed));
        skill.Cast(transform, keyUsed);
        cooldownTimers[skill] = skill.cooldown;

        yield return new WaitForSeconds(0.1f);
        while (anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName(skill.animationName) &&
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        IsCastingSkill = false;
        if (player != null) player.canControl = true;
    }

    private AudioClip GetClipByKey(KeyCode key)
    {
        return key switch
        {
            KeyCode.F => F,
            KeyCode.Q => Q,
            KeyCode.W => W,
            KeyCode.E => E,
            KeyCode.R => R,
            _ => null,
        };
    }

    private int GetSkillIndex(SkillSO skill)
    {
        if (skill == qSkill) return 0;
        if (skill == wSkill) return 1;
        if (skill == eSkill) return 2;
        if (skill == rSkill) return 3;
        return -1;
    }

    public float GetCooldown(SkillSO skill)
    {
        if (skill != null && cooldownTimers.TryGetValue(skill, out var t))
            return Mathf.Max(0f, t);
        return 0f;
    }

    public static IEnumerator DisableAttackRoutine(float duration)
    {
        canAttack = false;
        yield return new WaitForSeconds(duration);
        canAttack = true;
    }
}
