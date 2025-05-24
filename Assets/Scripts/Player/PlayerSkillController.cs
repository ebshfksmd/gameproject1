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

    private bool canAttack = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        foreach (var so in new[] { qSkill, wSkill, eSkill, rSkill })
            if (so != null)
                cooldownTimers[so] = 0f;
    }

    void Update()
    {
        // ��ٿ� ����
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
        if (requireCanAttack && !canAttack) return;          // ���� ��������� ����
        if (!cooldownTimers.ContainsKey(skill))
            cooldownTimers[skill] = 0f;

        if (Input.GetKeyDown(key) && cooldownTimers[skill] <= 0f)
            StartCoroutine(CastRoutine(skill));

    }

    private IEnumerator CastRoutine(SkillSO skill)
    {

        // 1) ���� �ִ� ���
        if (!string.IsNullOrEmpty(skill.animationName))
            anim.Play(skill.animationName);
        // 2) ���� ���
        yield return new WaitForSeconds(skill.castTime);


        // 3) ���� ��ų ȿ��
        skill.Cast(transform);

        // 4) ��Ÿ�� �ʱ�ȭ
        cooldownTimers[skill] = skill.cooldown;
    }


    public float GetCooldown(SkillSO skill)
    {
        if (skill != null && cooldownTimers.TryGetValue(skill, out var t))
            return Mathf.Max(0f, t);
        return 0f;
    }

    private IEnumerator DisableAttackRoutine(float duration)
    {
        canAttack = false;
        yield return new WaitForSeconds(duration);
        canAttack = true;
    }
}
