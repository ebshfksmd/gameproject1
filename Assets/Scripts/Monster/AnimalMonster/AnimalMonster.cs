using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimalMonster : Monster
{


    //기본공격 범위
    [SerializeField] float baseAtkDistance;
    //몇초마다 기본공격을 할지
    [SerializeField] float baseAtkCoolTime;

    //공격 준비중인지
    private bool prepareAtk=false;




    public IEnumerator BaseBasicAtk()
    {
        yield return new WaitForSeconds(baseAtkCoolTime);
        //플레이어로부터의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //공격하는 시점에 기본공격 범위안에 플레이어가 없다면 공격하지않음
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        prepareAtk = false;
    }

    

    private void Update()
    {
        //플레이어로부터의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //기본공격범위 안으로 들어왔을때
        if (distanceToTarget < baseAtkDistance && prepareAtk==false)
        {
            prepareAtk = true;
            StartCoroutine(BaseBasicAtk());
        }
    }
}
