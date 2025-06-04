using System.Collections;
using UnityEngine;

public class M_Centipede : Human
{
    //��ų �����ð�
    float skillCastingTime = 6f;
    //��ų ����
    int skillPower = 2;

    //��ų ��Ÿ�� ī��Ʈ
    float skillCount = 0;

    //��ų�� ĳ���õǰ��ִ��� Ȯ��
    bool isSkillCasting = false;


    //��ų�� �غ�Ǿ�����
    bool isSKillPrepare = true;

    //��ų ����
    float skillDistance = 3f;

    //�� ������
    IEnumerator PoisonDmg()
    {
        PlayerTest.instance.status = PlayerTest.Status.cantAtk;
        //
        for (int i = 0; i < 6; i++)
        {
            PlayerTest.instance.GetAttacked2(atk, skillPower);
            yield return new WaitForSeconds(1f);

        }
        PlayerTest.instance.status = PlayerTest.Status.basic;


    }

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(PoisonDmg());
            StartCoroutine(PlayerSkillController.DisableAttackRoutine(6f));
        }
        animator.SetBool("isWalk", true);
    }


    //��ų ����
    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float tempSpeed = speed;
        speed = 0f;
        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(skillCastingTime);

        StartCoroutine(SkillAnimation());

        speed = tempSpeed;
        isSKillPrepare = false;
        isSkillCasting = false;
    }




    //��ų�� ����� ��Ȳ���� �Ǵ��ϴ� �ڷ�ƾ
    System.Collections.IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);



            //��ų ��Ÿ���� �� ���� �ʾ�����
            if (isSKillPrepare == false)
            {
                skillCount += Time.deltaTime;
            }

            //��ų��Ÿ���� ��������
            if (skillCount >= skillCoolTime)
            {
                isSKillPrepare = true;
                skillCount = 0;
            }

            //�÷��̾ �����ȿ� ��������
            if (distanceToTarget < skillDistance && isSKillPrepare && !isSkillCasting)
            {
                // Ȥ�ö� �ߺ� ������ �� �����Ƿ� ���⼭�� �ѹ� �� ����
                isSkillCasting = true;
                StartCoroutine(SkillCast());
            }


            yield return null;
        }


    }

    private bool isSkillCheckRunning = false;

    public override void Skill()
    {
        if (!isSkillCheckRunning)
        {
            isSkillCheckRunning = true;
            StartCoroutine(SkillCheck());
        }
    }


#pragma warning disable CS0108 // ����� ��ӵ� ����� ����ϴ�. new Ű���尡 �����ϴ�.
#pragma warning disable CS0114 
    private void Awake()
#pragma warning restore CS0108 // ����� ��ӵ� ����� ����ϴ�. new Ű���尡 �����ϴ�.
#pragma warning restore CS0114
    {
        base.Awake();
        hp = 300;
        atk = 150;
        def = 100;
        type = MonsterType.human;

        Skill();
    }
}
