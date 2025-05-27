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

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            if (PlayerTest.instance != null)
            {
                PlayerTest.instance.GetAttacked2(atk, skillPower);
            }
            else
            {
                Debug.LogWarning("PlayerTest.instance is null. Skill attack canceled.");
            }
        }
    }

    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float tempSpeed = speed;
        speed = 0f;

        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(skillCastingTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(SkillAnimation());
        }

        animator.SetBool("isWalk", true);
        speed = tempSpeed;

        isSkillPrepared = false;
        isSkillCasting = false;
    }


    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!isSkillPrepared)
            {
                skillCount += Time.deltaTime;
            }

            if (skillCount >= skillCooltime)
            {
                isSkillPrepared = true;
                skillCount = 0f;
            }

            if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting)
            {
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }

    public override void Skill()
    {
        StartCoroutine(SkillCheck());
    }

#pragma warning disable CS0108
    private void Awake()
#pragma warning restore CS0108
    {
        base.Awake();

        hp = 100;
        atk = 15;
        def = 0;
        moveDistance = 5000f;
        type = MonsterType.animal;

        Skill();
    }
}
