using UnityEngine;

[CreateAssetMenu(fileName = "AOESkill", menuName = "Skills/AOE Skill")]
public class AOESkillSO : SkillSO
{
    [Header("AOE Settings")]                // 범위 반경
    public int damage = 30;                   // 데미지
    public int power = 3;
    public LayerMask enemyLayer;              // 적 레이어 마스크
    public GameObject effectPrefab;           // 시전 이펙트(옵션)

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        Debug.Log($"AOE Cast! 위치: {caster.position}, 반경: {range}");
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, range, enemyLayer);

        Debug.Log($"감지된 콜라이더 수: {hits.Length}");
        foreach (var col in hits)
        {
            Debug.Log($" Hit: {col.gameObject.name}");
            var enemy = col.GetComponent<Monster>();
            if (enemy != null)
            {
                enemy.GetAttacked(damage, power);
                Debug.Log($"  → {col.name} 에게 {damage} 데미지");
            }
        }
    }

}
