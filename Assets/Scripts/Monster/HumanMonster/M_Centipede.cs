using UnityEngine;

public class M_Centipede : Monster
{
    private void Awake()
    {
        hp = 300;
        atk = 150;
        def = 100;
        type = MonsterType.human;
    }
}
