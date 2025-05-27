using UnityEngine;

[CreateAssetMenu(fileName = "AOESkill", menuName = "Skills/AOE Skill")]
public class AOESkillSO : SkillSO
{
    [Header("AOE Settings")]                // ���� �ݰ�
    public int damage = 30;                   // ������
    public int power = 3;
    public LayerMask enemyLayer;              // �� ���̾� ����ũ
    public GameObject effectPrefab;           // ���� ����Ʈ(�ɼ�)

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        Debug.Log($"AOE Cast! ��ġ: {caster.position}, �ݰ�: {range}");
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, range, enemyLayer);

        Debug.Log($"������ �ݶ��̴� ��: {hits.Length}");
        foreach (var col in hits)
        {
            Debug.Log($" Hit: {col.gameObject.name}");
            var enemy = col.GetComponent<Monster>();
            if (enemy != null)
            {
                enemy.GetAttacked(damage, power);
                Debug.Log($"  �� {col.name} ���� {damage} ������");
            }
        }
    }

}
