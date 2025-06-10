using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Skills/TeamStatBuff")]
public class TeamStatBuffSkillSO : SkillSO
{
    [Header("Buff Settings")]
    [Tooltip("팀원들에게 더해줄 방어력 수치")]
    public int defenseAmount = 20;

    [Tooltip("스킬 사용 즉시 치유할 체력량")]
    public int healAmount = 50;

    [Tooltip("버프 지속 시간(초)")]
    public float duration = 20f;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // MonoBehaviour를 통해 코루틴 실행
        var mb = caster.GetComponent<MonoBehaviour>();
        if (mb != null)
            mb.StartCoroutine(ApplyTeamBuffs());
        else
            Debug.LogWarning($"[{name}] Cast: MonoBehaviour를 찾을 수 없어 버프를 적용할 수 없습니다.");
    }

    private IEnumerator ApplyTeamBuffs()
    {
        // 1) PlayerTest.AllPlayers에서 모든 플레이어 찾기
        var members = PlayerTest.AllPlayers;

        // 2) 버프 적용 및 즉시 치유
        foreach (var m in members)
        {
            if (defenseAmount != 0)
                m.BuffDefense(defenseAmount);

            if (healAmount != 0)
                m.Heal(healAmount);
        }

        // 3) 버프 지속 시간 대기
        yield return new WaitForSeconds(duration);

        // 4) 버프만 제거 (치유는 되돌리지 않습니다)
        foreach (var m in members)
        {
            if (defenseAmount != 0)
                m.BuffDefense(-defenseAmount);
        }
    }
}
