using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "AOEdoubleSkill", menuName = "Skills/AOE Double")]
public class AOEdoubleSkillSO : SkillSO
{
    [Header("AOE Settings")]
    public int damage = 30;
    public int power = 3;
    public int repeatCount = 2;
    public float delayBetween = 0.2f;    // 0.2�� ����
    public LayerMask enemyLayer;
    public GameObject effectPrefab;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // caster�� ���� MonoBehaviour (��: PlayerSkillController)�� �ڷ�ƾ ����
        var runner = caster.GetComponent<MonoBehaviour>();
        if (runner != null)
            runner.StartCoroutine(DoAOECoroutine(caster.position));
        else
            Debug.LogWarning("Cannot start coroutine: no MonoBehaviour on caster!");
    }

    private IEnumerator DoAOECoroutine(Vector3 center)
    {
        for (int i = 0; i < repeatCount; i++)
        {

            var hits = Physics2D.OverlapCircleAll(center, range, enemyLayer);
            foreach (var col in hits)
            {
                var m = col.GetComponent<Monster>();
                if (m != null)
                    m.GetAttacked(damage, power);
            }

            // ������ �ݺ��� �ƴϸ� ��� ���
            if (i < repeatCount - 1)
                yield return new WaitForSeconds(delayBetween);
        }
    }
}
