using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour
{
    //체력
    protected float hp;
    //공격력
    protected float atk;
    //방어력
    protected float def;
    //이동속도
    protected float speed;

    //게임 오프젝트
    public GameObject GameObject;

    //몬스터 타입 ( 걷는지 나는지 )
    public bool canFly = false;

    public IObjectPool<GameObject> Pool { get; set; }
    public void GetAttacked(float dmg,float power)
    {
        hp -= (dmg/(100+def))*power;
    }


}
