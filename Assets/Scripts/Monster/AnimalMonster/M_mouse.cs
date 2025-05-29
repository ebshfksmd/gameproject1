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
    Coroutine skillRoutine;

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
                Debug.Log("[��] �÷��̾�� ��ų ����!");
                PlayerTest.instance.GetAttacked2(atk, skillPower); // ���� ������ ����
            }
            else
            {
                Debug.LogWarning("PlayerTest.instance is null. Attack canceled.");
            }
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

        animator.SetBool("isWalk", true);
        speed = prevSpeed;

        isSkillPrepared = false;
        isSkillCasting = false;
    }

    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            //Debug.Log($"[�� ����] �Ÿ�: {distanceToTarget}, ��ų����: {isSkillPrepared}, ������: {isSkillCasting}");

            if (!isSkillPrepared)
            {
                skillCount += Time.deltaTime;
                if (skillCount >= skillCooltime)
                {
                    isSkillPrepared = true;
                    skillCount = 0f;
                }
            }

            if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting)
            {
                Debug.Log("[��] ��ų ���� ����");
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }


    public override void Skill()
    {
        if (skillRoutine == null)
        {
            skillRoutine = StartCoroutine(SkillCheck());
        }
    }

#pragma warning disable CS0108
    public override void Awake()
    {
        base.Awake();

        hp = 100;
        atk = 15;
        def = 0;
        moveDistance = 5000f;
        type = MonsterType.animal;

        Skill(); // Start skill loop
    }

}
