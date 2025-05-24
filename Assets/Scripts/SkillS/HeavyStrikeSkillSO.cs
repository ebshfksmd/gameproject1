using UnityEngine;

[CreateAssetMenu(fileName = "HeavyStrikeSkill", menuName = "Skills/Heavy Strike")]
public class HeavyStrikeSkillSO : SkillSO
{
    [Header("Heavy Strike Settings")]
    [Tooltip("가하는 데미지")]
    public int damage = 100;
    public int power = 3;
    [Tooltip("적 레이어")]
    public LayerMask enemyLayer;
    [Tooltip("타격 이펙트(선택)")]
    public GameObject effectPrefab;

    public override void Cast(Transform caster)
    {
        // 1) 반경 내 모든 적 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, range, enemyLayer);

        // 2) 체력이 가장 많은 적 찾기
        Monster target = null;
        int maxHP = -1;
        foreach (var col in hits)
        {
            var eh = col.GetComponent<Monster>();
            if (eh != null && eh.CurrentHealth > maxHP)
            {
                maxHP = eh.CurrentHealth;
                target = eh;
            }
        }

        // 3) 타격 처리
        if (target != null)
        {
            // (선택) 이펙트 재생
            if (effectPrefab != null)
                Instantiate(effectPrefab,
                            target.transform.position,
                            Quaternion.identity);

            // 데미지
            target.GetAttacked(damage, power);
        }
    }
}