using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealLowestHealth")]
public class HealLowestHealthSkillSO : SkillSO
{
    [Header("Heal Settings")]
    [Tooltip("치유량")]
    public int healAmount = 50;

    public override void Cast(Transform caster)
    {
        // 씬에 있는 모든 Player_health 컴포넌트 찾기
        var members = Object.FindObjectsOfType<Player_health>();
        Player_health target = null;

        // 가장 체력이 낮은 아군 선택
        foreach (var m in members)
        {
            if (target == null || m.CurrentHealth < target.CurrentHealth)
                target = m;
        }

        // 대상이 있으면 치유
        if (target != null)
            target.Heal(healAmount);
    }
}