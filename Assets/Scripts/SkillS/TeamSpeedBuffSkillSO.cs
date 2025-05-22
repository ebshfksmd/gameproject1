using UnityEngine;

[CreateAssetMenu(fileName = "TeamSpeedBuffSkill", menuName = "Skills/Team Speed Buff")]
public class TeamSpeedBuffSkillSO : SkillSO
{
    [Header("Buff Settings")]
    public float multiplier = 2f;    // 속도 배수
    public float duration = 30f;   // 지속 시간
    public GameObject effectPrefab;  // 시전 이펙트 (옵션)

    public override void Cast(Transform caster)
    {
        if (effectPrefab != null)
            Instantiate(effectPrefab, caster.position, Quaternion.identity);

        // 씬에 존재하는 모든 Player 컴포넌트를 찾아서 버프
        Player[] allPlayers = GameObject.FindObjectsOfType<Player>();
        foreach (var p in allPlayers)
        {
            p.ApplySpeedBuff(multiplier, duration);
        }
    }
}
