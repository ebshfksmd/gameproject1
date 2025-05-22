using UnityEngine;

[CreateAssetMenu(fileName = "StatBuffSkill", menuName = "Skills/Stat Buff")]
public class StatBuffSkillSO : SkillSO
{

    [Header("Buff Parameters")]
    [SerializeField] private float healthPercent = 0.3f;

    [Tooltip("추가 회복량")]
    [SerializeField] private int healAmount = 10;

    [Tooltip("방어력 증가량")]
    [SerializeField] private int defenseAmount = 20;


    public override void Cast(Transform caster)
    {

        var ph = caster.GetComponent<Player_health>();
        if (ph != null)
        {
            // 최대체력 + 회복
            ph.BuffMaxHealth(healthPercent);

            // 즉시 회복, 최대체력 넘지 않도록 Clamp
            ph.Heal(healAmount);

            // 방어력 버프
            ph.BuffDefense(defenseAmount);
        }
    }
}
