using UnityEngine;

[CreateAssetMenu(fileName = "AOESkill", menuName = "Skills/AOE Skill")]
public class AOESkillSO : SkillSO
{
    [Header("AOE Settings")]                // ���� �ݰ�
    public int damage = 30;                   // ������
    public LayerMask enemyLayer;              // �� ���̾� ����ũ
    public GameObject effectPrefab;           // ���� ����Ʈ(�ɼ�)

    public override void Cast(Transform caster)
    {
        Debug.Log($"AOE Cast! ��ġ: {caster.position}, �ݰ�: {range}");
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, range, enemyLayer);

        Debug.Log($"������ �ݶ��̴� ��: {hits.Length}");
        foreach (var col in hits)
        {
            Debug.Log($" Hit: {col.gameObject.name}");
            var enemy = col.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"  �� {col.name} ���� {damage} ������");
            }
        }
    }

}
