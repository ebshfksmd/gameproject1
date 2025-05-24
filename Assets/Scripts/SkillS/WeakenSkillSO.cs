using UnityEngine;

[CreateAssetMenu(fileName = "WeakenSkill", menuName = "Skills/WeakenSkillSO")]
public class WeakenSkillSO : SkillSO
{
    [Header("Weaken Settings")]
    public float radius = 5f;                  // Ž�� �ݰ�
    [Range(0f, 1f)] public float reducePercent = 0.2f; // ���ݷ� ���� ����
    public float duration = 5f;                // ����� ���� �ð�
    public LayerMask enemyLayer;               // �� ���̾�
    public GameObject effectPrefab;            // ��Ʈ ����Ʈ(�ɼ�)

    public override void Cast(Transform caster)
    {
        // (�ɼ�) ����Ʈ
        if (effectPrefab != null)
            Instantiate(effectPrefab, caster.position, Quaternion.identity);

        // �ݰ� �� ��� �� Ž��
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, radius, enemyLayer);

        // ���� ����� �� ã��
        Collider2D nearest = null;
        float minDistSqr = float.MaxValue;
        foreach (var hit in hits)
        {
            float d = (hit.transform.position - caster.position).sqrMagnitude;
            if (d < minDistSqr)
            {
                minDistSqr = d;
                nearest = hit;
            }
        }

        if (nearest != null)
        {
            var enemy = nearest.GetComponent<Monster>();
            if (enemy != null)
                enemy.ApplyAttackDebuff(reducePercent, duration);
        }
    }
}