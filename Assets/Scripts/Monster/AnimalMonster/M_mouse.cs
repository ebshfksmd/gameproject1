using System.Collections;
using UnityEngine;

public class M_mouse : Animal
{
    float skillCooltime = 8f;
    float skillCastingTime = 3f;
    float skillDistance = 3f;
    int skillPower = 4;

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
                Debug.Log("[쥐] 스킬 준비 완료");
            }
        }

        // 스킬 우선 시전
        if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting)
        {
            Debug.Log("[쥐] 스킬 시전 시작");
            StartCoroutine(SkillCast());
            return;
        }

        // 기본 공격
        if (distanceToTarget < baseAtkDistance && !prepareAtk && !isSkillCasting)
        {
            prepareAtk = true;
            StartCoroutine(AnimalBaseBasicAtk());
        }

        // 사망 처리
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
        if (distanceToTarget < skillDistance)
        {
            if (PlayerTest.instance != null)
            {
                Debug.Log($"[스킬공격] {gameObject.name}이(가) {skillPower} 피해를 입힘");
                PlayerTest.instance.GetAttacked2(atk, skillPower);
            }
            else
            {
                Debug.LogWarning("[M_mouse] PlayerTest.instance is null");
            }
        }

        animator.SetBool("isSkill", false);
    }

    public virtual void Awake()
    {
        base.Awake();

        hp = 100;
        atk = 150;
        def = 0;
        moveDistance = 5000f;
        type = MonsterType.animal;
    }
}
