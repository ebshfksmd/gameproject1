using UnityEngine;
using System.Collections;

public class MonsterCombat : MonoBehaviour
{
    [Header("공통 설정")]
    public Transform target;
    public Animator animator;
    public float speed = 1f;
    public float attackDistance = 2f;

    [Header("기본 공격")]
    public float basicAttackCooldown = 2f;
    public int basicAttackDamage = 5;

    [Header("스킬 공격")]
    public float skillCooldown = 5f;
    public float skillCastingTime = 1.5f;
    public float skillRange = 3f;
    public int skillDamage = 10;

    private float basicCooldownTimer = 0f;
    private float skillCooldownTimer = 0f;
    private bool isSkillCasting = false;

    void Update()
    {
        if (target == null || isSkillCasting) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // 우선 스킬 검사
        if (skillCooldownTimer <= 0f && distance < skillRange)
        {
            StartCoroutine(SkillRoutine());
            return;
        }

        // 기본 공격 검사
        if (basicCooldownTimer <= 0f && distance < attackDistance)
        {
            StartCoroutine(BasicAttackRoutine());
        }

        // 쿨타임 갱신
        if (skillCooldownTimer > 0f) skillCooldownTimer -= Time.deltaTime;
        if (basicCooldownTimer > 0f) basicCooldownTimer -= Time.deltaTime;
    }

    IEnumerator BasicAttackRoutine()
    {
        animator.SetTrigger("BasicAttack");
        basicCooldownTimer = basicAttackCooldown;

        yield return new WaitForSeconds(0.3f); // 타이밍 맞춰서 데미지 적용

        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            Debug.Log("[기본공격] " + basicAttackDamage + " 피해를 입힘");
            PlayerTest.instance?.GetAttacked(0, basicAttackDamage);
        }
    }

    IEnumerator SkillRoutine()
    {
        isSkillCasting = true;
        animator.SetTrigger("Skill");

        yield return new WaitForSeconds(skillCastingTime);

        if (Vector3.Distance(transform.position, target.position) < skillRange)
        {
            Debug.Log("[스킬공격] " + skillDamage + " 피해를 입힘");
            PlayerTest.instance?.GetAttacked2(0, skillDamage);
        }

        skillCooldownTimer = skillCooldown;
        isSkillCasting = false;
    }
}
