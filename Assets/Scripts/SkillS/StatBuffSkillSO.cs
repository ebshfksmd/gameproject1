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


    public override void Cast(Transform caster)
    {

        var ph = caster.GetComponent<Player_health>();
        if (ph != null)
        {
            // �ִ�ü�� + ȸ��
            ph.BuffMaxHealth(healthPercent);

            // ��� ȸ��, �ִ�ü�� ���� �ʵ��� Clamp
            ph.Heal(healAmount);

            // ���� ����
            ph.BuffDefense(defenseAmount);
        }
    }
}
