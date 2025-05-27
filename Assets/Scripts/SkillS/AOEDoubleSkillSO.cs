using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "AOEdoubleSkill", menuName = "Skills/AOE Double")]
public class AOEdoubleSkillSO : SkillSO
{
    [Header("AOE Settings")]
    public int damage = 30;
    public int power = 3;
    public int repeatCount = 2;
    public float delayBetween = 0.2f;    // 0.2초 간격
    public LayerMask enemyLayer;
    public GameObject effectPrefab;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        // caster에 붙은 MonoBehaviour (예: PlayerSkillController)로 코루틴 실행
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

            // 마지막 반복이 아니면 잠깐 대기
            if (i < repeatCount - 1)
                yield return new WaitForSeconds(delayBetween);
        }
    }
}
