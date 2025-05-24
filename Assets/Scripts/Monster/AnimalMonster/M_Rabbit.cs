using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class M_Rabbit : Animal
{
    //스킬 쿨타임
    float skillCooltime = 15f;
    //스킬 시전시간
    float skillCastingTime = 3f;
    //스킬 범위
    float skillDistance = 3f;
    //스킬 위력
    int skillPower = 6;

    //스킬 쿨타임 카운트
    float skillCount = 0;

    //스킬이 캐스팅되고있는지 확인
    bool isSkillCasting = false;

    //스킬이 준비되었는지
    bool isSKillPrepare = true;

    //스킬 시전
    IEnumerator SkillCast()
    {
        isSkillCasting = true;


        //뒤로 물러날때 플레이어를 따라가는것을 방지
        float tempSpeed = speed;
        speed = 0;


        //초당 0.2m씩 뒤로 물러나기
        for (int i = 0; i < skillCastingTime; i++)
        {
            transform.position += Vector3.right * moveDirection * 0.2f * (-1f);
            yield return new WaitForSeconds(1f);
        }


        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //물러난 시점에서 스킬범위 체크하고 스킬시전
        if (distanceToTarget <= skillDistance)
        {
            transform.position += Vector3.right * moveDirection * 0.8f ;
            PlayerTest.instance.GetAttacked2(atk, skillPower);
        }
        isSKillPrepare = false;
        isSkillCasting = false;

        //스킬을 사용해준후에 몬스터의 스피드 원상복구
        speed=tempSpeed;
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
        atk = 15;
        hp = 100;
        def = 30;
        type = MonsterType.animal;
        Skill();
    }
}
