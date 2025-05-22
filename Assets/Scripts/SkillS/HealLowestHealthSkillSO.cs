using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HealLowestHealth")]
public class HealLowestHealthSkillSO : SkillSO
{
    [Header("Heal Settings")]
    [Tooltip("ġ����")]
    public int healAmount = 50;

    public override void Cast(Transform caster)
    {
        // ���� �ִ� ��� Player_health ������Ʈ ã��
        var members = Object.FindObjectsOfType<Player_health>();
        Player_health target = null;

        // ���� ü���� ���� �Ʊ� ����
        foreach (var m in members)
        {
            if (target == null || m.CurrentHealth < target.CurrentHealth)
                target = m;
        }

        // ����� ������ ġ��
        if (target != null)
            target.Heal(healAmount);
    }
}