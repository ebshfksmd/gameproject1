using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealLowestHealth")]
public class HealLowestHealthSkillSO : SkillSO
{
    [Header("Heal Settings")]
    [Tooltip("치유량")]
    public int healAmount = 50;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // 씬에 있는 모든 PlayerTest 검색 (정렬 없음: 성능 ↑)
        var members = Object.FindObjectsByType<PlayerTest>(FindObjectsSortMode.None);
        PlayerTest target = null;

        foreach (var m in members)
        {
            if (m.hp <= 0) continue;

            if (target == null || m.hp < target.hp)
                target = m;
        }

        if (target != null)
        {
            Debug.Log($"[HealSkill] {target.name}에게 {healAmount} 힐 시전.");
            target.Heal(healAmount);
        }
    }
}
