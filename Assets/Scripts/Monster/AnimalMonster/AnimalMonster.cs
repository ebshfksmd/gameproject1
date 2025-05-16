using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimalMonster : Monster
{
    // 동물형 몬스터 클래스로 옮기기
    [SerializeField] float baseAtkDistance;
    [SerializeField] float baseAtkCoolTime;


    //부모 클래스에서 호출할 코루틴
    IEnumerator BaseBasicAtk()
    {
        yield return new WaitForSeconds(baseAtkCoolTime);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            player.GetAttacked(atk, baseAttackPower);
        }
    }

    public void BasicAttack()
    {
        StartCoroutine(BaseBasicAtk());
    }

}
