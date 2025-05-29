using System.Collections;
using UnityEngine;

public class M_Rabbit : Animal
{
    float skillCooltime = 15f;
    float skillCastingTime = 3f;
    float skillDistance = 3f;
    int skillPower = 6;

    float skillCount = 0;
    bool isSkillCasting = false;
    bool isSkillPrepare = true;

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);

        if (target == null)
        {
            Debug.LogWarning("[Rabbit] Target is null.");
            yield break;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget <= skillDistance)
        {
            transform.position += Vector3.right * moveDirection * 0.8f;

            if (PlayerTest.instance != null)
            {
                Debug.Log($"[Rabbit] 공격 성공: {skillPower} 데미지");
                PlayerTest.instance.GetAttacked2(atk, skillPower);
            }
            else
            {
                Debug.LogWarning("[Rabbit] PlayerTest.instance is null.");
            }
        }
    }

    IEnumerator SkillCast()
    {
        isSkillCasting = true;

        float tempSpeed = speed;
        speed = 0;

        for (int i = 0; i < skillCastingTime; i++)
        {
            transform.position += Vector3.right * moveDirection * 0.2f * -1f;
            yield return new WaitForSeconds(1f);
        }

        isSkillPrepare = false;
        isSkillCasting = false;

        StartCoroutine(SkillAnimation());

        speed = tempSpeed;
    }

    IEnumerator SkillCheck()
    {
        while (true)
        {
            if (target == null)
            {
                Debug.LogWarning("[Rabbit] 타겟이 없습니다.");
                yield return new WaitForSeconds(1f);
                continue;
            }

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!isSkillPrepare)
            {
                skillCount += Time.deltaTime;
            }

            if (skillCount >= skillCooltime)
            {
                isSkillPrepare = true;
                skillCount = 0;
                Debug.Log("[Rabbit] 스킬 준비 완료");
            }

            if (distanceToTarget < skillDistance && isSkillPrepare && !isSkillCasting)
            {
                Debug.Log("[Rabbit] 스킬 시전 시작");
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }

    public override void Skill()
    {
        Debug.Log("[Rabbit] 스킬 코루틴 시작");
        StartCoroutine(SkillCheck());
    }

    public override void Awake()
    {
        base.Awake();
        atk = 15;
        hp = 100;
        def = 30;
        type = MonsterType.animal;
        Skill();
    }
}
