using UnityEngine;

[CreateAssetMenu(fileName = "WeakenSkill", menuName = "Skills/WeakenSkillSO")]
public class WeakenSkillSO : SkillSO
{
    [Header("Weaken Settings")]
    public float radius = 5f;                  // 탐지 반경
    [Range(0f, 1f)] public float reducePercent = 0.2f; // 공격력 감소 비율
    public float duration = 5f;                // 디버프 지속 시간
    public LayerMask enemyLayer;               // 적 레이어
    public GameObject effectPrefab;            // 히트 이펙트(옵션)

    public override void Cast(Transform caster)
    {
        // (옵션) 이펙트
        if (effectPrefab != null)
            Instantiate(effectPrefab, caster.position, Quaternion.identity);

        // 반경 내 모든 적 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, radius, enemyLayer);

        // 가장 가까운 적 찾기
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