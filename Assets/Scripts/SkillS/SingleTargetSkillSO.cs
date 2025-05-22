using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetSkill", menuName = "Skills/Single Target")]
public class SingleTargetSkillSO : SkillSO
{
    [Header("Single Target Settings")]
    public int damage = 20;
    public int power = 3;// 입힐 데미지
    public LayerMask enemyLayer;             // Enemy 레이어 마스크
    public GameObject effectPrefab;          // (선택) 타격 이펙트 프리팹

    public override void Cast(Transform caster)
    {
        Vector2 origin = caster.position;
        // localScale.x 의 부호(+1/-1)로 전방 방향 결정
        float dirSign = Mathf.Sign(caster.localScale.x);
        Vector2 forward = new Vector2(dirSign, 0f);

        // 반경 내 모든 적 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, enemyLayer);

        Collider2D nearest = null;
        float minDistSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector2 toEnemy = (hit.transform.position - caster.position);
            // “전방” 체크: forward 벡터와의 내적이 양수일 때만
            if (Vector2.Dot(forward, toEnemy.normalized) <= 0f)
                continue;

            float distSqr = toEnemy.sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                nearest = hit;
            }
        }

        if (nearest != null)
        {
            // 이펙트
            if (effectPrefab != null)
            {
                var fx = Instantiate(effectPrefab, nearest.transform.position, Quaternion.identity);
                // 이펙트도 바라보는 방향에 맞춰 뒤집고 싶다면:
                fx.transform.localScale = new Vector3(dirSign * Mathf.Abs(fx.transform.localScale.x),
                                                      fx.transform.localScale.y,
                                                      fx.transform.localScale.z);
            }

            // EnemyHealth가 있으면 데미지
            var enemy = nearest.GetComponent<Monster>();
            if (enemy != null)
                enemy.GetAttacked(damage,power);
        }
    }
}
