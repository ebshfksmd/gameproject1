using UnityEngine;

[CreateAssetMenu(fileName = "TeamSpeedBuffSkill", menuName = "Skills/Team Speed Buff")]
public class TeamSpeedBuffSkillSO : SkillSO
{
    [Header("Buff Settings")]
    public float multiplier = 2f;    // �ӵ� ���
    public float duration = 30f;   // ���� �ð�
    public GameObject effectPrefab;  // ���� ����Ʈ (�ɼ�)

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        if (effectPrefab != null)
            Instantiate(effectPrefab, caster.position, Quaternion.identity);

        // ���� �����ϴ� ��� Player ������Ʈ�� ã�Ƽ� ����
        Player[] allPlayers = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (var p in allPlayers)
        {
            p.ApplySpeedBuff(multiplier, duration);
        }
    }
}
