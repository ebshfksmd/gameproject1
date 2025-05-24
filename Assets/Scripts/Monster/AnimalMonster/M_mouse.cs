using System.Collections;
using UnityEngine;

public class M_mouse : Animal
{

    //스킬 쿨타임
    float skillCooltime = 8f;
    //스킬 시전시간
    float skillCastingTime = 3f;
    //스킬 범위
    float skillDistance = 3f;

    //스킬 위력
    int skillPower = 4;

    //스킬 쿨타임 카운트
    float skillCount = 0;

    //스킬이 캐스팅되고있는지 확인
    bool isSkillCasting = false;


    //스킬이 준비되었는지
    bool isSKillPrepare = true;


    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);
    }


    //스킬 시전
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



    //스킬을 사용할 상황인지 판단하는 코루틴
    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);



            //스킬 쿨타임이 다 돌지 않았을때
            if (isSKillPrepare == false)
            {
                skillCount += Time.deltaTime;
            }

            //스킬쿨타임이 돌았을때
            if (skillCount >= skillCooltime)
            {
                isSKillPrepare = true;
                skillCount = 0;
            }

            //플레이어가 범위안에 들어왔을때
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


#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
    private void Awake()
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
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
