using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class M_Rabbit : Animal
{
    //��ų ��Ÿ��
    float skillCooltime = 15f;
    //��ų �����ð�
    float skillCastingTime = 3f;
    //��ų ����
    float skillDistance = 3f;
    //��ų ����
    int skillPower = 6;

    //��ų ��Ÿ�� ī��Ʈ
    float skillCount = 0;

    //��ų�� ĳ���õǰ��ִ��� Ȯ��
    bool isSkillCasting = false;

    //��ų�� �غ�Ǿ�����
    bool isSKillPrepare = true;


    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill",true );
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill",false);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //������ �������� ��ų���� üũ�ϰ� ��ų����
        // ������ �������� ���� ���⼭ ��ġ���
        if (distanceToTarget <= 5)
        {
            transform.position += Vector3.right * moveDirection * 0.8f;
            //PlayerTest.instance.GetAttacked2(atk, skillPower);
        }
    }

    //��ų ����
    IEnumerator SkillCast()
    {
        isSkillCasting = true;


        //�ڷ� �������� �÷��̾ ���󰡴°��� ����
        float tempSpeed = speed;
        speed = 0;


        //�ʴ� 0.2m�� �ڷ� ��������
        for (int i = 0; i < skillCastingTime; i++)
        {
            transform.position += Vector3.right * moveDirection * 0.2f * (-1f);
            yield return new WaitForSeconds(1f);
        }


       
        isSKillPrepare = false;
        isSkillCasting = false;

        StartCoroutine(SkillAnimation());
        //��ų�� ��������Ŀ� ������ ���ǵ� ���󺹱�
        speed = tempSpeed;
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
