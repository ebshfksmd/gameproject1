using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/TeamStatBuff")]
public class TeamStatBuffSkillSO : SkillSO
{
    [Header("Buff Settings")]
    [Tooltip("�����鿡�� ������ ���� ��ġ")]
    public int defenseAmount = 20;

    [Tooltip("��ų ��� ��� ġ���� ü�·�")]
    public int healAmount = 50;

    [Tooltip("���� ���� �ð�(��)")]
    public float duration = 20f;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // MonoBehaviour�� ���� �ڷ�ƾ ����
        var mb = caster.GetComponent<MonoBehaviour>();
        if (mb != null)
            mb.StartCoroutine(ApplyTeamBuffs());
        else
            Debug.LogWarning($"[{name}] Cast: MonoBehaviour�� ã�� �� ���� ������ ������ �� �����ϴ�.");
    }

    private IEnumerator ApplyTeamBuffs()
    {
        // 1) PlayerTest.AllPlayers���� ��� �÷��̾� ã��
        var members = PlayerTest.AllPlayers;

        // 2) ���� ���� �� ��� ġ��
        foreach (var m in members)
        {
            if (defenseAmount != 0)
                m.BuffDefense(defenseAmount);

            if (healAmount != 0)
                m.Heal(healAmount);
        }

        // 3) ���� ���� �ð� ���
        yield return new WaitForSeconds(duration);

        // 4) ������ ���� (ġ���� �ǵ����� �ʽ��ϴ�)
        foreach (var m in members)
        {
            if (defenseAmount != 0)
                m.BuffDefense(-defenseAmount);
        }
    }
}
