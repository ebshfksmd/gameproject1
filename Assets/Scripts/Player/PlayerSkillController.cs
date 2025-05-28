using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private Dictionary<SkillSO, float> cooldownTimers = new Dictionary<SkillSO, float>();
    private Animator anim;
    private Player player;
    private AudioSource skillAudioSource;

    // ��ų ĳ���� ���� Ȯ�ο�
    public static bool IsCastingSkill { get; private set; } = false;

    [HideInInspector]
    public static bool canAttack = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();

        skillAudioSource = gameObject.AddComponent<AudioSource>();
        skillAudioSource.loop = false;
        skillAudioSource.playOnAwake = false;
        skillAudioSource.volume = 1f;

        foreach (var so in new[] { qSkill, wSkill, eSkill, rSkill })
        {
            if (so != null)
                cooldownTimers[so] = 0f;
        }
    }

    void Update()
    {
        // ��Ÿ�� ���� �� �α� ���
        var keys = new List<SkillSO>(cooldownTimers.Keys);
        foreach (var so in keys)
        {
            if (cooldownTimers[so] > 0f)
            {
                cooldownTimers[so] -= Time.deltaTime;
                Debug.Log($"[{so.skillName}] ��Ÿ�� ����: {cooldownTimers[so]:F1}��");
            }
        }

        // ��ų ���� �õ�
        TryCast(KeyCode.F, attack);
        TryCast(KeyCode.Q, qSkill);
        TryCast(KeyCode.W, wSkill);
        TryCast(KeyCode.E, eSkill);
        TryCast(KeyCode.R, rSkill);

        // ĳ���� ���� �� �ִϸ��̼� ����
        if (IsCastingSkill && anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRun", false);
        }
    }

    private void TryCast(KeyCode key, SkillSO skill)
    {
        if (skill == null) return;
        if (!canAttack || IsCastingSkill) return;

        if (!cooldownTimers.ContainsKey(skill))
            cooldownTimers[skill] = 0f;

        if (Input.GetKeyDown(key) && cooldownTimers[skill] <= 0f)
        {
            StartCoroutine(CastRoutine(skill, key));
        }
    }

    private IEnumerator CastRoutine(SkillSO skill, KeyCode keyUsed)
    {
        if (player != null) player.canControl = false;
        IsCastingSkill = true;

        // �ִϸ��̼� ����
        if (!string.IsNullOrEmpty(skill.animationName) && anim != null)
        {
            anim.Play(skill.animationName);
        }

        // ����� ����
        AudioClip clip = GetClipByKey(keyUsed);
        if (clip != null)
        {
            skillAudioSource.Stop();
            skillAudioSource.clip = clip;
            skillAudioSource.Play();
        }

        // ��ų ȿ�� ����
        skill.Cast(transform, keyUsed);
        cooldownTimers[skill] = skill.cooldown;

        float waitTime = Mathf.Max(skill.castTime, clip != null ? clip.length : 0f);
        yield return new WaitForSeconds(waitTime);

        if (player != null) player.canControl = true;
        IsCastingSkill = false;
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
