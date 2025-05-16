using UnityEngine;




//테스트용으로 임의로 만든 플레이어 스크립트
public class PlayerTest
{
    public int atk;
    public int def;
    public int hp;


    public void GetAttacked(int dmg, int power)
    {
        hp -= (dmg / (100 + def)) * power;
    }

}
