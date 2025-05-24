using UnityEngine;

[CreateAssetMenu(fileName = "HeavyStrikeSkill", menuName = "Skills/Heavy Strike")]
public class HeavyStrikeSkillSO : SkillSO
{
    [Header("Heavy Strike Settings")]
    [Tooltip("���ϴ� ������")]
    public int damage = 100;
    public int power = 3;
    [Tooltip("�� ���̾�")]
    public LayerMask enemyLayer;
    [Tooltip("Ÿ�� ����Ʈ(����)")]
    public GameObject effectPrefab;

    public override void Cast(Transform caster)
    {
        // 1) �ݰ� �� ��� �� Ž��
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, range, enemyLayer);

        // 2) ü���� ���� ���� �� ã��
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

        // 3) Ÿ�� ó��
        if (target != null)
        {
            // (����) ����Ʈ ���
            if (effectPrefab != null)
                Instantiate(effectPrefab,
                            target.transform.position,
                            Quaternion.identity);

            // ������
            target.GetAttacked(damage, power);
        }
    }
}