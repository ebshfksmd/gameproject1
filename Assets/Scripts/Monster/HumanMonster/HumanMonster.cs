using System.Collections;
using UnityEngine;

public class HumanMonster : Monster
{


    //기본공격범위
    [SerializeField] float baseAtkDistance;
    //몇초마다 공격할지 ( 계속 안에 있어야함 )
    [SerializeField] float baseAtkCoolTime;
    //시전시간 ( 공격할 시점에서 범위 안에있으면됨 )
    [SerializeField] float castingTime;



    public IEnumerator BaseBasicAtk()
    {
        yield return new WaitForSeconds(castingTime);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            //PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
    }



    //범위안에 얼마만큼 머물러있었는지 확인하는 변수
    float count=0;

    //공격시전이 시작되었는지 확인하는 변수
    bool isCast=false;



    private void Update()
    {
        //플레이어와의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        //기본공격 범위안에 들어왔을때
        if (distanceToTarget < baseAtkDistance)
        {
            //캐스팅중이 아닐때 범위안에 얼마만큼 있었는지 카운트
            if(!isCast)
            {
                count += Time.deltaTime;
            }
        }
        else
        {
            //공격범위를 벗어나면 머무는 시간을 다시 잼
            count = 0;
        }

        // 기본공격 범위안에 ~초동안 머물렀을때
        if(count >= baseAtkCoolTime)
        {
            StartCoroutine(BaseBasicAtk());
            isCast = true;
            count = 0;
            
        }
    }
}
