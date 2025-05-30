using UnityEngine;

[CreateAssetMenu(fileName = "StatBuffSkill", menuName = "Skills/Stat Buff")]
public class StatBuffSkillSO : SkillSO
{
    [Header("Buff Parameters")]
    [SerializeField] private float healthPercent = 0.3f;

    [Tooltip("�߰� ȸ����")]
    [SerializeField] private int healAmount = 10;

    [Tooltip("���� ������")]
    [SerializeField] private int defenseAmount = 20;

    public override void Cast(Transform caster, KeyCode keyUsed)
    {
        var pt = caster.GetComponent<PlayerTest>();
        if (pt != null)
        {
            // �ִ� ü�� ����
            int extraMaxHp = Mathf.RoundToInt(pt.MaxHp * healthPercent);
            pt.MaxHp += extraMaxHp;

            // ȸ�� (�ִ� ü���� ���� �ʰ� Clamp)
            pt.Heal(healAmount);

            // ���� ����
            pt.def += defenseAmount;
        }
    }
}
