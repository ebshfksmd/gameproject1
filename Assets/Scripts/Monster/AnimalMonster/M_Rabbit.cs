using System.Collections;
using UnityEngine;

public class M_Rabbit : Animal
{
    float skillCastingTime = 3f;
    float skillDistance = 3f;
    int skillPower = 6;

    float skillCount = 0f;
    bool isSkillCasting = false;
    bool isSkillPrepared = true;

    [Header("Rabbit SFX")]
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip skillClip;
    [SerializeField] private AudioClip rushClip;
    [SerializeField] private AudioClip jumpClip;

    protected override void Update()
    {
        if (isDead) return;

        UpdateHpBar();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        HandleMovement(distanceToTarget);

        if (!isSkillPrepared)
        {
            skillCount += Time.deltaTime;
            if (skillCount >= skillCoolTime)
            {
                isSkillPrepared = true;
                skillCount = 0f;
                Debug.Log("[Rabbit] 스킬 준비 완료");
            }
        }

        if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting && !isStun)
        {
            StartCoroutine(SkillCast());
        }

        if (!isSkillPrepared && !isSkillCasting && distanceToTarget < baseAtkDistance && !prepareAtk)
        {
            prepareAtk = true;
            StartCoroutine(AnimalBaseBasicAtk());
        }

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

        // 스킬 사운드: 토끼 점프
        if (jumpClip != null)
            SoundManager.Instance.SFXPlay("토끼 점프", jumpClip);

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
        // 제거된 SkillCheck 로직 대체 불필요
    }

 IEnumerator AnimalBaseBasicAtk()
    {
        animator.SetBool("isAttack", true);

        // 기본 공격 사운드: 스킬
        if (skillClip != null)
            SoundManager.Instance.SFXPlay("스킬", skillClip);

        yield return new WaitForSeconds(baseAtkAnimationTime);

        if (PlayerTest.instance != null)
        {
            PlayerTest.instance.GetAttacked(atk,skillPower);
        }

        animator.SetBool("isAttack", false);
        prepareAtk = false;
    }

IEnumerator DieAnimation()
    {
        isDead = true;
        speed = 0f;
        animator.SetBool("isDie", true);

        // 쓰러짐 + 죽음 사운드 재생
        if (dieClip != null)
            SoundManager.Instance.SFXPlay("die", dieClip);

        yield return new WaitForSeconds(0.3f);
        Destroy(hpBarInstance?.gameObject);
        hpBarInstance = null;
        Destroy(gameObject);
    }
}
