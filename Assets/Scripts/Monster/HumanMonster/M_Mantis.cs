using System.Collections;
using UnityEngine;

public class M_Mantis : Monster
{
    ///��ų ��Ÿ��
    float skillCooltime = 20f;
    //��ų �����ð�
    float skillCastingTime = 2f;
    //��ų ����
    float skillDistance = 8f;
    //��ų ����
    int skillPower = 15;

    //��ų ��Ÿ�� ī��Ʈ
    float skillCount = 0;

    //��ų�� ĳ���õǰ��ִ��� Ȯ��
    bool isSkillCasting = false;


    //��ų�� �غ�Ǿ�����
    bool isSKillPrepare = true;


    //��ų ����
    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        yield return new WaitForSeconds(skillCastingTime);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            PlayerTest.instance.GetAttacked2(atk, skillPower);
        }


        isSKillPrepare = false;
        isSkillCasting = false;
    }



    //��ų�� ����� ��Ȳ���� �Ǵ��ϴ� �ڷ�ƾ
    IEnumerator SkillCheck()
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
        atk = 150;
        hp = 300;
        def = 150;
        type = MonsterType.human;
        Skill();
    }


}
