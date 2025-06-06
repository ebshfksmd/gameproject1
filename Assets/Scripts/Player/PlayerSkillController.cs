using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Assign Skills")]
    public SkillSO attack;
    public SkillSO qSkill;
    public SkillSO wSkill;
    public SkillSO eSkill;
    public SkillSO rSkill;

    [Header("Audio")]
    public AudioClip F;
    public AudioClip Q;
    public AudioClip W;
    public AudioClip E;
    public AudioClip R;

    [Header("Cooldown UI")]
    public Image[] skillCooldownFills = new Image[4]; // Q, W, E, R
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
        skillAudioSource.loop = false;
        skillAudioSource.playOnAwake = false;
        skillAudioSource.volume = 1f;

        foreach (var so in new[] { qSkill, wSkill, eSkill, rSkill })
        {
            if (so != null)
                cooldownTimers[so] = 0f;
        }

        for (int i = 0; i < skillCooldownFills.Length; i++)
        {
            if (skillCooldownFills[i] != null)
                skillCooldownFills[i].gameObject.SetActive(false);

            if (skillCooldownTexts[i] != null)
                skillCooldownTexts[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (dialogueManager == null)
            dialogueManager = FindObjectOfType<DialogueManager>();

        if (!canAttack || (dialogueManager != null && dialogueManager.IsDialogueActive()))
            return;
        var keys = new List<SkillSO>(cooldownTimers.Keys);
        foreach (var so in keys)
        {
            int index = GetSkillIndex(so);
            if (cooldownTimers[so] > 0f)
            {
                cooldownTimers[so] -= Time.deltaTime;
                float remaining = Mathf.Max(0f, cooldownTimers[so]);
                float cooldownTime = so.cooldown;

                if (index >= 0 && index < skillCooldownFills.Length)
                {
                    if (skillCooldownFills[index] != null)
                    {
                        skillCooldownFills[index].gameObject.SetActive(true);
                        skillCooldownFills[index].fillAmount = Mathf.Clamp01(remaining / cooldownTime);
                    }

                    if (skillCooldownTexts[index] != null)
                    {
                        skillCooldownTexts[index].gameObject.SetActive(true);
                        skillCooldownTexts[index].text = $"{remaining:F1}";
                    }
                }
            }
            else
            {
                if (index >= 0 && index < skillCooldownFills.Length)
                {
                    if (skillCooldownFills[index] != null)
                        skillCooldownFills[index].gameObject.SetActive(false);

                    if (skillCooldownTexts[index] != null)
                        skillCooldownTexts[index].gameObject.SetActive(false);
                }
            }
        }

        TryCast(KeyCode.F, attack);
        TryCast(KeyCode.Q, qSkill);
        TryCast(KeyCode.W, wSkill);
        TryCast(KeyCode.E, eSkill);
        TryCast(KeyCode.R, rSkill);

        if (IsCastingSkill && anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRun", false);
        }
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
        {
            StartCoroutine(CastRoutine(skill, key));
        }
    }

    private IEnumerator CastRoutine(SkillSO skill, KeyCode keyUsed)
    {
        if (player != null)
            player.canControl = false;

        IsCastingSkill = true;

        if (!string.IsNullOrEmpty(skill.animationName) && anim != null)
        {
            anim.Play(skill.animationName);
        }

        AudioClip clip = GetClipByKey(keyUsed);
        if (clip != null)
        {
            skillAudioSource.Stop();
            skillAudioSource.clip = clip;
            skillAudioSource.Play();
        }
        skillAudioSource.PlayOneShot(GetClipByKey(keyUsed));
        skill.Cast(transform, keyUsed);
        cooldownTimers[skill] = skill.cooldown;

        if (!string.IsNullOrEmpty(skill.animationName) && anim != null)
        {
            yield return null;
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            while (state.IsName(skill.animationName) && state.normalizedTime < 1f)
            {
                yield return null;
                state = anim.GetCurrentAnimatorStateInfo(0);
            }
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
