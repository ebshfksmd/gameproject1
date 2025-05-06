using UnityEngine;
using UnityEngine.Pool;

public class Monster : MonoBehaviour
{
    //체력
    protected int hp;
    //공격력
    protected int atk;
    //방어력
    protected int def;
    //이동속도
    protected float speed;

    //게임 오프젝트
    public GameObject GameObject;

    public IObjectPool<GameObject> Pool { get; set; }
    public void GetAttacked(int dmg)
    {
        hp -= dmg - def;
    }
}
