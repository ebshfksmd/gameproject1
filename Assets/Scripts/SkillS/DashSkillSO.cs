using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/DashSkill")]
public class DashSkillSO : SkillSO
{
    [Header("Dash Settings (��� ����)")]
    [Tooltip("��� ���� �ð�(��)")]
    public float dashDuration = 0.2f;

    // SkillSO�� 'range' �ʵ带 ��� �Ÿ��� ����մϴ�

    [Header("Damage Settings")]
    [Tooltip("��� �� ������ ���� ������")]
    public int dashDamage = 20;
    [Tooltip("������ ������ ���� �ݰ�")]
    public float hitRadius = 0.5f;
    [Tooltip("������ ������ ����� ���̾� ����ũ")]
    public LayerMask enemyLayer;
    [Tooltip("Ÿ�� ȿ�� ������(����)")]
    public GameObject effectPrefab;

    public override void Cast(Transform caster)
    {
        var mb = caster.GetComponent<MonoBehaviour>();
        if (mb != null)
            mb.StartCoroutine(DashCoroutine(caster));
        else
            Debug.LogWarning($"[{name}] Cast: MonoBehaviour�� ã�� �� ���� ��ø� ������ �� �����ϴ�.");
    }

    private IEnumerator DashCoroutine(Transform caster)
    {
        var rb = caster.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        // 1) ���� ��ġ�� ��ǥ ��ġ ���
        float dir = Mathf.Sign(caster.localScale.x);
        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + Vector2.right * dir * range;

        // 2) �߷� ��� ����
        float origGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // �ߺ� ��Ʈ ������ ���� ����
        var hitSet = new HashSet<Collider2D>();

        // 3) ����ϸ鼭 �̵� �� ������ ����
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);

            // ������ ����
            Collider2D[] hits = Physics2D.OverlapCircleAll(newPos, hitRadius, enemyLayer);
            Debug.Log($"Dash hit {hits.Length} targets at pos {newPos}");
            foreach (var hit in hits)
            {
                if (!hitSet.Contains(hit))
                {
                    hitSet.Add(hit);
                    var enemy = hit.GetComponent<EnemyHealth>();
                    if (enemy != null)
                    {
                        Debug.Log($"  �� Damaging {hit.name} for {dashDamage}");
                        enemy.TakeDamage(dashDamage);
                    }
                }
            }


            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4) ������ ��ǥ ��ġ�� �̵�
        rb.MovePosition(targetPos);

        // 5) �߷� ����
        rb.gravityScale = origGravity;
    }
}
