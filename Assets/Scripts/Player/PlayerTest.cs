using System.Collections;
using UnityEngine;




//테스트용으로 임의로 만든 플레이어 스크립트
public class PlayerTest: MonoBehaviour 
{
    public static PlayerTest instance;



    public int atk=10;
    public int def=1;
    public int hp=100;
    [SerializeField] SpriteRenderer sprite1;
    [SerializeField] SpriteRenderer sprite2;

    public enum Status
    {
        basic,
        cantAtk
    }

    public Status status=Status.basic; 

    IEnumerator Hit()
    {
        sprite1.color = Color.red;
        sprite2.color = Color.red;
        yield return new WaitForSeconds(1f);
        sprite1.color = Color.white;
        sprite2.color = Color.white;
    }
    
    IEnumerator Hit2()
    {
        sprite1.color = Color.blue;
        sprite2.color = Color.blue;
        yield return new WaitForSeconds(1f);
        sprite1.color = Color.white;
        sprite2.color = Color.white;
    }


    public void GetAttacked(int dmg, int power)
    {

        Debug.Log("맞음");

        hp -= (dmg / (100 + def)) * power;
        StartCoroutine(Hit());
        
    }
    
    public void GetAttacked2(int dmg, int power)
    {

        Debug.Log("쳐맞음");

        hp -= (dmg / (100 + def)) * power;
        StartCoroutine(Hit2());
        
    }

    private void Awake()
    {
        instance = this;
    }


}
