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

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        var pt = caster.GetComponent<PlayerTest>();
        if (pt != null)
        {
            // 최대 체력 증가
            int extraMaxHp = Mathf.RoundToInt(pt.MaxHp * healthPercent);
            pt.MaxHp += extraMaxHp;

            // 회복 (최대 체력을 넘지 않게 Clamp)
            pt.Heal(healAmount);

            // 방어력 증가
            pt.def += defenseAmount;
        }
    }
}
