using System.Collections;
using UnityEngine;

public class M_mouse : Animal
{

    //��ų ��Ÿ��
    float skillCooltime = 8f;
    //��ų �����ð�
    float skillCastingTime = 3f;
    //��ų ����
    float skillDistance = 3f;

    //��ų ����
    int skillPower = 4;

    //��ų ��Ÿ�� ī��Ʈ
    float skillCount = 0;

    //��ų�� ĳ���õǰ��ִ��� Ȯ��
    bool isSkillCasting = false;


    //��ų�� �غ�Ǿ�����
    bool isSKillPrepare = true;


    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);
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
        animator.SetBool("isWalk", true);
        speed = tempSpeed;
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

        hp = 100;
        atk = 15;
        def = 0;
        moveDistance = 5000f;
        type = MonsterType.animal;
        Skill();
    }




}
