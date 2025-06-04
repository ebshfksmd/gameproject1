using System.Collections;
using UnityEngine;

public class M_Centipede : Human
{
    //스킬 시전시간
    float skillCastingTime = 6f;
    //스킬 위력
    int skillPower = 2;

    //스킬 쿨타임 카운트
    float skillCount = 0;

    //스킬이 캐스팅되고있는지 확인
    bool isSkillCasting = false;


    //스킬이 준비되었는지
    bool isSKillPrepare = true;

    //스킬 범위
    float skillDistance = 3f;

    //독 데미지
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


    //스킬 시전
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




    //스킬을 사용할 상황인지 판단하는 코루틴
    System.Collections.IEnumerator SkillCheck()
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
            if (skillCount >= skillCoolTime)
            {
                isSKillPrepare = true;
                skillCount = 0;
            }

            //플레이어가 범위안에 들어왔을때
            if (distanceToTarget < skillDistance && isSKillPrepare && !isSkillCasting)
            {
                // 혹시라도 중복 진입할 수 있으므로 여기서도 한번 더 차단
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


#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
#pragma warning disable CS0114 
    private void Awake()
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
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
