using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skills/DashSkill")]
public class DashSkillSO : SkillSO
{
    [Header("Dash Settings (대시 전용)")]
    [Tooltip("대시 지속 시간(초)")]
    public float dashDuration = 0.2f;

    // SkillSO의 'range' 필드를 대시 거리로 사용합니다

    [Header("Damage Settings")]
    [Tooltip("대시 중 적에게 입힐 데미지")]
    public int dashDamage = 20;
    [Tooltip("데미지 판정을 위한 반경")]
    public float hitRadius = 0.5f;
    [Tooltip("데미지 판정에 사용할 레이어 마스크")]
    public LayerMask enemyLayer;
    [Tooltip("타격 효과 프리팹(선택)")]
    public GameObject effectPrefab;

    public override void Cast(Transform caster)
    {
        var mb = caster.GetComponent<MonoBehaviour>();
        if (mb != null)
            mb.StartCoroutine(DashCoroutine(caster));
        else
            Debug.LogWarning($"[{name}] Cast: MonoBehaviour를 찾을 수 없어 대시를 실행할 수 없습니다.");
    }

    private IEnumerator DashCoroutine(Transform caster)
    {
        var rb = caster.GetComponent<Rigidbody2D>();
        if (rb == null) yield break;

        // 1) 시작 위치와 목표 위치 계산
        float dir = Mathf.Sign(caster.localScale.x);
        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + Vector2.right * dir * range;

        // 2) 중력 잠시 해제
        float origGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // 중복 히트 방지를 위한 집합
        var hitSet = new HashSet<Collider2D>();

        // 3) 대시하면서 이동 및 데미지 판정
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            Vector2 newPos = Vector2.Lerp(startPos, targetPos, t);
            rb.MovePosition(newPos);

            // 데미지 판정
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
                        Debug.Log($"  → Damaging {hit.name} for {dashDamage}");
                        enemy.TakeDamage(dashDamage);
                    }
                }
            }


            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4) 끝까지 목표 위치로 이동
        rb.MovePosition(targetPos);

        // 5) 중력 복원
        rb.gravityScale = origGravity;
    }
}
