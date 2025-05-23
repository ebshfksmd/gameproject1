using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("Assign Skills")]
    public SkillSO attack;
    public SkillSO qSkill;
    public SkillSO wSkill;
    public SkillSO eSkill;
    public SkillSO rSkill;

    private Dictionary<SkillSO, float> cooldownTimers = new Dictionary<SkillSO, float>();

    private Animator anim;
    private Player player;

    [HideInInspector]
    public static bool canAttack = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        foreach (var so in new[] { qSkill, wSkill, eSkill, rSkill })
            if (so != null)
                cooldownTimers[so] = 0f;
    }

    void Update()
    {
        // 쿨다운 감소
        var keys = new List<SkillSO>(cooldownTimers.Keys);
        foreach (var so in keys)
            if (cooldownTimers[so] > 0f)
                cooldownTimers[so] -= Time.deltaTime;

        TryCast(KeyCode.F, attack);
        TryCast(KeyCode.Q, qSkill);
        TryCast(KeyCode.W, wSkill);
        TryCast(KeyCode.E, eSkill);
        TryCast(KeyCode.R, rSkill);
    }

    private void TryCast(KeyCode key, SkillSO skill)
    {
        if (skill == null) return;
        if (!canAttack) return;          // 공격 잠겨있으면 무시
        if (!cooldownTimers.ContainsKey(skill))
            cooldownTimers[skill] = 0f;

        if (Input.GetKeyDown(key) && cooldownTimers[skill] <= 0f)
            StartCoroutine(CastRoutine(skill));

    }

    private IEnumerator CastRoutine(SkillSO skill)
    {
        if (player != null) player.canControl = false;

        // 1) 시전 애니 재생
        if (!string.IsNullOrEmpty(skill.animationName))
            anim.Play(skill.animationName);
        // 2) 시전 대기
        yield return new WaitForSeconds(skill.castTime);


        // 3) 실제 스킬 효과
        skill.Cast(transform);

        // 4) 쿨타임 초기화
        cooldownTimers[skill] = skill.cooldown;

        // 5) 시전 후 이동 허용
        if (player != null) player.canControl = true;
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
