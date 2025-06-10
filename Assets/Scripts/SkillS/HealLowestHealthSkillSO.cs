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
            Debug.Log("��� Ǯü��!");
            return;
        }

        var target = members[0];
        Debug.Log($"{target.name}({target.hp}/{target.MaxHp})��(��) {healAmount}��ŭ ġ��");
        target.Heal(healAmount);
    }
}