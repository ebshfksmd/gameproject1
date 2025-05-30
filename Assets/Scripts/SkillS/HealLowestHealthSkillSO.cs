using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealLowestHealth")]
public class HealLowestHealthSkillSO : SkillSO
{
    [Header("Heal Settings")]
    [Tooltip("ġ����")]
    public int healAmount = 50;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // ���� �ִ� ��� PlayerTest �˻� (���� ����: ���� ��)
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
            Debug.Log($"[HealSkill] {target.name}���� {healAmount} �� ����.");
            target.Heal(healAmount);
        }
    }
}
