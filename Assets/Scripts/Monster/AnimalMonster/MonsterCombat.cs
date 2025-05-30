using UnityEngine;
using System.Collections;

public class MonsterCombat : MonoBehaviour
{
    [Header("���� ����")]
    public Transform target;
    public Animator animator;
    public float speed = 1f;
    public float attackDistance = 2f;

    [Header("�⺻ ����")]
    public float basicAttackCooldown = 2f;
    public int basicAttackDamage = 5;

    [Header("��ų ����")]
    public float skillCooldown = 5f;
    public float skillCastingTime = 1.5f;
    public float skillRange = 3f;
    public int skillDamage = 10;

    private float basicCooldownTimer = 0f;
    private float skillCooldownTimer = 0f;
    private bool isSkillCasting = false;

    void Update()
    {
        if (target == null || isSkillCasting) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // �켱 ��ų �˻�
        if (skillCooldownTimer <= 0f && distance < skillRange)
        {
            StartCoroutine(SkillRoutine());
            return;
        }

        // �⺻ ���� �˻�
        if (basicCooldownTimer <= 0f && distance < attackDistance)
        {
            StartCoroutine(BasicAttackRoutine());
        }

        // ��Ÿ�� ����
        if (skillCooldownTimer > 0f) skillCooldownTimer -= Time.deltaTime;
        if (basicCooldownTimer > 0f) basicCooldownTimer -= Time.deltaTime;
    }

    IEnumerator BasicAttackRoutine()
    {
        animator.SetTrigger("BasicAttack");
        basicCooldownTimer = basicAttackCooldown;

        yield return new WaitForSeconds(0.3f); // Ÿ�̹� ���缭 ������ ����

        if (Vector3.Distance(transform.position, target.position) < attackDistance)
        {
            Debug.Log("[�⺻����] " + basicAttackDamage + " ���ظ� ����");
            PlayerTest.instance?.GetAttacked(0, basicAttackDamage);
        }
    }

    IEnumerator SkillRoutine()
    {
        isSkillCasting = true;
        animator.SetTrigger("Skill");

        yield return new WaitForSeconds(skillCastingTime);

        if (Vector3.Distance(transform.position, target.position) < skillRange)
        {
            Debug.Log("[��ų����] " + skillDamage + " ���ظ� ����");
            PlayerTest.instance?.GetAttacked2(0, skillDamage);
        }

        skillCooldownTimer = skillCooldown;
        isSkillCasting = false;
    }
}
