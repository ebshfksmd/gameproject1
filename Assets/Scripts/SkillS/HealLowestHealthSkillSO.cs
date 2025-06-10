using System.Linq;    
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealLowestHealth")]
public class HealLowestHealthSkillSO : SkillSO
{
    [Header("Heal Settings")]
    public int healAmount = 50;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {

        var members = PlayerTest.AllPlayers
                        .Where(m => m.hp < m.MaxHp)   
                        .OrderBy(m => m.hp)
                        .ToArray();
        if (members.Length == 0)
        {
            Debug.Log("모두 풀체력!");
            return;
        }

        var target = members[0];
        Debug.Log($"{target.name}({target.hp}/{target.MaxHp})을(를) {healAmount}만큼 치유");
        target.Heal(healAmount);
    }
}