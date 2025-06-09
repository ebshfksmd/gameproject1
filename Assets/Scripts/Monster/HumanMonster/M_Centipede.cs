using System.Collections;
using UnityEngine;

public class M_Centipede : Human
{
    // ��ų ����
    float skillCastingTime = 6f;
    int skillPower = 2;
    float skillCount = 0;
    bool isSkillCasting = false;
    bool isSKillPrepare = true;
    float skillDistance = 3f;

    // ���� Ŭ��
    [Header("���� Ŭ��")]
    public AudioClip basicSound;
    public AudioClip baseAttackSound;
    public AudioClip skillSound;

    private bool isSkillCheckRunning = false;

    public override void Awake()
    {
        base.Awake();
        hp = 300;
        atk = 150;
        def = 100;
        type = MonsterType.human;
        Skill();
    }

    // ��ų �˻� ����
    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!isSKillPrepare)
            {
                skillCount += Time.deltaTime;
                if (skillCount >= skillCoolTime)
                {
                    isSKillPrepare = true;
                    skillCount = 0;
                }
            }

            if (distanceToTarget < skillDistance && isSKillPrepare && !isSkillCasting)
            {
                isSkillCasting = true;
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }

    // ��ų ����
    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float tempSpeed = speed;
        speed = 0f;
        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(skillCastingTime);

        StartCoroutine(SkillAnimation());

        speed = tempSpeed;
        isSKillPrepare = false;
        isSkillCasting = false;
    }

    // ��ų �ִϸ��̼�
    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);

        // ��ų ���� ���
        if (skillSound != null)
            SoundManager.Instance.SFXPlay("��ų", skillSound);

        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(PoisonDmg());
            StartCoroutine(PlayerSkillController.DisableAttackRoutine(6f));
        }

        animator.SetBool("isWalk", true);
    }

    // ���� ������
    IEnumerator PoisonDmg()
    {
        PlayerTest.instance.status = PlayerTest.Status.cantAtk;

        // �⺻���� ���� ���
        if (baseAttackSound != null)
            SoundManager.Instance.SFXPlay("�⺻����", baseAttackSound);

        for (int i = 0; i < 6; i++)
        {
            PlayerTest.instance.GetAttacked2(atk, skillPower);
            yield return new WaitForSeconds(1f);
        }

        PlayerTest.instance.status = PlayerTest.Status.basic;
    }

    public override void Skill()
    {
        if (!isSkillCheckRunning)
        {
            isSkillCheckRunning = true;
            StartCoroutine(SkillCheck());
        }
    }

    // �Ϲ� ���� ���� ��� (������ ��� ����)
    void Update()
    {
        if (!isDead && basicSound != null && !SoundManager.Instance.IsSFXPlaying(basicSound))
        {
            SoundManager.Instance.SFXPlay("�⺻", basicSound);
        }
    }
}
