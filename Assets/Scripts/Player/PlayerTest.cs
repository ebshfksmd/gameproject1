using System.Collections;
using UnityEngine;




//�׽�Ʈ������ ���Ƿ� ���� �÷��̾� ��ũ��Ʈ
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

        Debug.Log("����");

        hp -= (dmg / (100 + def)) * power;
        StartCoroutine(Hit());
        
    }
    
    public void GetAttacked2(int dmg, int power)
    {

        Debug.Log("�ĸ���");

        hp -= (dmg / (100 + def)) * power;
        StartCoroutine(Hit2());
        
    }

    private void Awake()
    {
        instance = this;
    }


}
