using UnityEngine;

[CreateAssetMenu(fileName = "SingleTargetSkill", menuName = "Skills/Single Target")]
public class SingleTargetSkillSO : SkillSO
{
    [Header("Single Target Settings")]
    public int damage = 20;
    public int power = 3;// ���� ������
    public LayerMask enemyLayer;             // Enemy ���̾� ����ũ
    public GameObject effectPrefab;          // (����) Ÿ�� ����Ʈ ������

    public override void Cast(Transform caster)
    {
        Vector2 origin = caster.position;
        // localScale.x �� ��ȣ(+1/-1)�� ���� ���� ����
        float dirSign = Mathf.Sign(caster.localScale.x);
        Vector2 forward = new Vector2(dirSign, 0f);

        // �ݰ� �� ��� �� Ž��
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, enemyLayer);

        Collider2D nearest = null;
        float minDistSqr = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector2 toEnemy = (hit.transform.position - caster.position);
            // �����桱 üũ: forward ���Ϳ��� ������ ����� ����
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
            // ����Ʈ
            if (effectPrefab != null)
            {
                var fx = Instantiate(effectPrefab, nearest.transform.position, Quaternion.identity);
                // ����Ʈ�� �ٶ󺸴� ���⿡ ���� ������ �ʹٸ�:
                fx.transform.localScale = new Vector3(dirSign * Mathf.Abs(fx.transform.localScale.x),
                                                      fx.transform.localScale.y,
                                                      fx.transform.localScale.z);
            }

            // EnemyHealth�� ������ ������
            var enemy = nearest.GetComponent<Monster>();
            if (enemy != null)
                enemy.GetAttacked(damage,power);
        }
    }
}
