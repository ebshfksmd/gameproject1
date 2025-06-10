using System.Collections;
using UnityEngine;

public class M_Centipede : Human
{
    float skillCastingTime = 6f;
    int skillPower = 2;
    float skillCount = 0;
    bool isSkillCasting = false;
    bool isSkillPrepared = true;
    float skillDistance = 3f;

    [Header("����")]
    public AudioClip �⺻;         // �ȱ� �� �⺻ ���� ����
    public AudioClip �⺻����;     // �Ϲ� ���� Ÿ�̹�
    public AudioClip ��ų;         // ��ų Ÿ�̹�

    private bool isLoopingSoundPlaying = false;

    IEnumerator PoisonDmg()
    {
        PlayerTest.instance.status = PlayerTest.Status.cantAtk;

        for (int i = 0; i < 6; i++)
        {
            PlayerTest.instance.GetAttacked2(atk, skillPower);
            yield return new WaitForSeconds(1f);
        }

        PlayerTest.instance.status = PlayerTest.Status.basic;
    }

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);

        // ��ų ����
        if (��ų != null)
            SoundManager.Instance.SFXPlay("��ų", ��ų);

        yield return new WaitForSeconds(skillAtkAnimationTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(PoisonDmg());
            StartCoroutine(PlayerSkillController.DisableAttackRoutine(6f));
        }

        animator.SetBool("isSkill", false);
        animator.SetBool("isWalk", true);
    }

    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float tempSpeed = speed;
        speed = 0f;
        animator.SetBool("isWalk", false);
        StopLoopingSound();

        yield return new WaitForSeconds(skillCastingTime);

        StartCoroutine(SkillAnimation());

        speed = tempSpeed;
        isSkillPrepared = false;
        isSkillCasting = false;
    }

    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!isSkillPrepared)
                skillCount += Time.deltaTime;

            if (skillCount >= skillCoolTime)
            {
                isSkillPrepared = true;
                skillCount = 0;
            }

            if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting)
            {
                isSkillCasting = true;
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }

    private void StopLoopingSound()
    {
        if (�⺻ != null)
            SoundManager.Instance.StopLoopSound(�⺻);

        isLoopingSoundPlaying = false;
    }

    private void TryPlayLoopingSound()
    {
        if (!isLoopingSoundPlaying && �⺻ != null)
        {
            SoundManager.Instance.PlayLoopSound(�⺻);
            isLoopingSoundPlaying = true;
        }
    }

    private bool isSkillCheckRunning = false;

    public override void Skill()
    {
        if (!isSkillCheckRunning)
        {
            isSkillCheckRunning = true;
            StartCoroutine(SkillCheck());
        }
    }

    private void Awake()
    {
        base.Awake();
        hp = 300;
        atk = 150;
        def = 100;
        type = MonsterType.human;

        Skill();
    }

  void Update()
    {
        if (isDead) return;

        UpdateHpBar();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        if (isTracking)
        {
            transform.position += (target.position - transform.position).normalized * speed * Time.deltaTime;
            animator.SetBool("isWalk", true);
            TryPlayLoopingSound();
        }
        else
        {
            animator.SetBool("isWalk", false);
            StopLoopingSound();
        }

        if (hp <= 0 && !isDead)
        {
            StartCoroutine(DieAnimation());
        }
    }

    IEnumerator DieAnimation()
    {
        speed = 0f;
        animator.SetBool("isDie", true);

        // ���� ���嵵 ���� ���
        if (�⺻���� != null)
            SoundManager.Instance.SFXPlay("�⺻����", �⺻����);

        if (�⺻ != null)
            SoundManager.Instance.SFXPlay("�⺻", �⺻); // ������ �� ȿ��

        yield return new WaitForSeconds(0.3f);
        isDead = true;
        Destroy(hpBarInstance?.gameObject);
        hpBarInstance = null;
        Destroy(gameObject);
    }
}
