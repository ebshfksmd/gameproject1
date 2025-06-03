// Assets/Scripts/Skills/SingleSlowDefenseSkillSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "SingleSlowDefenseSkill", menuName = "Skills/Single Slow & Defense Debuff")]
public class SingleSlowDefenseSkillSO : SkillSO
{
    [Header("Debuff Settings")]
    [Tooltip("����� ���� �ݰ�")]
    public float radius = 5f;
    [Tooltip("�̵� �ӵ� ���� ���� (��: 0.3 = 30% ����)")]
    [Range(0f, 1f)] public float moveSpeedReducePercent = 0.3f;
    [Tooltip("���� ���ҷ�")]
    public int defenseReduceAmount = 2;
    [Tooltip("����� ���� �ð�")]
    public float duration = 5f;

    [Header("Targeting")]
    [Tooltip("�� ���̾� ����ũ")]
    public LayerMask enemyLayer;

    [Header("Visuals (Optional)")]
    [Tooltip("����Ʈ ������ (���� ����)")]
    public GameObject effectPrefab;
    public bool isJER=false;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // 1) ����Ʈ
        if (effectPrefab != null)
            Instantiate(effectPrefab, caster.position, Quaternion.identity);

        // 2) �ݰ� �� ��� �� Ž��
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            caster.position, radius, enemyLayer);

        // 3) ���� ����� �� ã��
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

        // 4) �� ���Ը� �����
        if (nearest != null)
        {
            var deb = nearest.GetComponent<Monster>();
            Debug.Log("����");
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
                    Debug.Log("��ö�̴�");
                    Player[] allPlayers = GameObject.FindObjectsByType<Player>(FindObjectsSortMode.None);

                    foreach (var p in allPlayers)
                    {
                        //ApplySpeedBuff(�̼��� ��ŭ ������ų��, ���ӽð�);
                        p.ApplySpeedBuff(1.4f, 10f);
                    }
                }
            }
        }
    }
}
