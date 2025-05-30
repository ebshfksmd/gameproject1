using System.Collections;
using UnityEngine;

public class M_Rabbit : Animal
{
    float skillCooltime = 15f;
    float skillCastingTime = 3f;
    float skillDistance = 3f;
    int skillPower = 6;

    float skillCount = 0f;
    bool isSkillCasting = false;
    bool isSkillPrepared = true;

    protected override void Update()
    {
        if (isDead) return;

        UpdateHpBar();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        HandleMovement(distanceToTarget);

        // 스킬 쿨타임 관리
        if (!isSkillPrepared)
        {
            skillCount += Time.deltaTime;
            if (skillCount >= skillCooltime)
            {
                isSkillPrepared = true;
                skillCount = 0f;
                Debug.Log("[Rabbit] 스킬 준비 완료");
            }
        }

        // 스킬 시전
        if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting)
        {
            StartCoroutine(SkillCast());
            return; // 스킬 시전 중에는 아무것도 하지 않음
        }

        // 평타 시도 (스킬 쿨타임 중일 때만)
        if (!isSkillPrepared && !isSkillCasting && distanceToTarget < baseAtkDistance && !prepareAtk)
        {
            prepareAtk = true;
            StartCoroutine(AnimalBaseBasicAtk());
        }

        // 사망
        if (hp <= 0 && !isDead)
        {
            StartCoroutine(DieAnimation());
        }
    }

    void HandleMovement(float distanceToTarget)
    {
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
        {
            isTracking = true;
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            animator.SetBool("isWalk", true);
            moveDirection = (transform.position.x > target.position.x) ? -1 : 1;
        }
        else if (distanceToTarget > targetDistance)
        {
            isTracking = false;
            animator.SetBool("isWalk", true);
            transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

            float distanceFromStart = transform.position.x - startPos.x;
            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1;
                startPos = transform.position;
            }
        }
        else
        {
            isTracking = false;
            animator.SetBool("isWalk", false);
        }
    }

    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float prevSpeed = speed;
        speed = 0f;

        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(skillCastingTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(SkillAnimation());
        }

        speed = prevSpeed;
        animator.SetBool("isWalk", true);

        isSkillPrepared = false;
        isSkillCasting = false;
    }

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= skillDistance)
        {
            transform.position += Vector3.right * moveDirection * 0.8f;

            if (PlayerTest.instance != null)
            {
                Debug.Log($"[스킬공격] {gameObject.name}이(가) {skillPower} 피해를 입힘");
                PlayerTest.instance.GetAttacked2(atk, skillPower);
            }
            else
            {
                Debug.LogWarning("[Rabbit] PlayerTest.instance is null.");
            }
        }

        animator.SetBool("isSkill", false);
    }

    public override void Awake()
    {
        base.Awake();
        atk = 15;
        hp = 100;
        def = 30;
        type = MonsterType.animal;
    }

    public override void Skill()
    {
        // 더 이상 필요하지 않음 (SkillCheck 제거했기 때문에)
    }
}
