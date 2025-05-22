using System.Collections;
using UnityEngine;

public class M_Centipede : Monster
{
    //��ų ��Ÿ��
    float skillCooltime = 20f;
    //��ų �����ð�
    float skillCastingTime = 6f;
    //��ų ����
    float skillDistance = 10f;
    //��ų ����
    int skillPower = 2;

    //��ų ��Ÿ�� ī��Ʈ
    float skillCount = 0;

    //��ų�� ĳ���õǰ��ִ��� Ȯ��
    bool isSkillCasting = false;


    //��ų�� �غ�Ǿ�����
    bool isSKillPrepare = true;



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



    //��ų ����
    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        yield return new WaitForSeconds(skillCastingTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(PoisonDmg());
        }

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
            if (skillCount >= skillCooltime)
            {
                isSKillPrepare = true;
                skillCount = 0;
            }

            //�÷��̾ �����ȿ� ��������
            if (distanceToTarget < skillDistance && isSKillPrepare == true && isSkillCasting == false)
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


#pragma warning disable CS0108 // ����� ��ӵ� ����� ����ϴ�. new Ű���尡 �����ϴ�.
    private void Awake()
#pragma warning restore CS0108 // ����� ��ӵ� ����� ����ϴ�. new Ű���尡 �����ϴ�.
    {
        base.Awake();
        hp = 300;
        atk = 150;
        def = 100;
        type = MonsterType.human;
        Skill();
    }
}
