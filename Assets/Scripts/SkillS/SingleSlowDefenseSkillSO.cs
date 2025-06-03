// Assets/Scripts/Skills/SingleSlowDefenseSkillSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "SingleSlowDefenseSkill", menuName = "Skills/Single Slow & Defense Debuff")]
public class SingleSlowDefenseSkillSO : SkillSO
{
    [Header("Debuff Settings")]
    [Tooltip("디버프 적용 반경")]
    public float radius = 5f;
    [Tooltip("이동 속도 감소 비율 (예: 0.3 = 30% 감소)")]
    [Range(0f, 1f)] public float moveSpeedReducePercent = 0.3f;
    [Tooltip("방어력 감소량")]
    public int defenseReduceAmount = 2;
    [Tooltip("디버프 지속 시간")]
    public float duration = 5f;

    [Header("Targeting")]
    [Tooltip("적 레이어 마스크")]
    public LayerMask enemyLayer;

    [Header("Visuals (Optional)")]
    [Tooltip("이펙트 프리팹 (생략 가능)")]
    public GameObject effectPrefab;
    public bool isJER=false;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // 1) 이펙트
        if (effectPrefab != null)
            Instantiate(effectPrefab, caster.position, Quaternion.identity);

        // 2) 반경 내 모든 적 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, radius, enemyLayer);

        // 3) 가장 가까운 적 찾기
        Collider2D nearest = null;
        float minDistSqr = float.MaxValue;
        foreach (var col in hits)
        {
            float d = (col.transform.position - caster.position).sqrMagnitude;
            if (d < minDistSqr)
            {
                minDistSqr = d;
                nearest = col;
            }
        }

        // 4) 한 명에게만 디버프
        if (nearest != null)
        {
            var deb = nearest.GetComponent<Monster>();
            Debug.Log("몬스터");
            if (deb != null)
            {
                deb.ApplyMoveSpeedDebuff(moveSpeedReducePercent, duration);
                if(isJER)
                {
                    deb.ApplyDefenseDebuff(defenseReduceAmount, duration);
                    Debug.Log("JER");
                }
                else
                {
                    Debug.Log("원철이니");
                    Player[] allPlayers = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);

                    foreach (var p in allPlayers)
                    {
                        //ApplySpeedBuff(이속을 얼만큼 증가시킬지, 지속시간);
                        p.ApplySpeedBuff(1.4f, 10f);
                    }
                }
            }
        }
    }
}
