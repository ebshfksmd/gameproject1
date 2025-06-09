using System.Collections;
using UnityEngine;

public class M_Centipede : Human
{
    // 스킬 관련
    float skillCastingTime = 6f;
    int skillPower = 2;
    float skillCount = 0;
    bool isSkillCasting = false;
    bool isSKillPrepare = true;
    float skillDistance = 3f;

    // 사운드 클립
    [Header("사운드 클립")]
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

    // 스킬 검사 루프
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

    // 스킬 시전
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

    // 스킬 애니메이션
    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);

        // 스킬 사운드 재생
        if (skillSound != null)
            SoundManager.Instance.SFXPlay("스킬", skillSound);

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

    // 지속 데미지
    IEnumerator PoisonDmg()
    {
        PlayerTest.instance.status = PlayerTest.Status.cantAtk;

        // 기본공격 사운드 재생
        if (baseAttackSound != null)
            SoundManager.Instance.SFXPlay("기본공격", baseAttackSound);

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

    // 일반 동작 사운드 재생 (선택적 사용 예시)
    void Update()
    {
        if (!isDead && basicSound != null && !SoundManager.Instance.IsSFXPlaying(basicSound))
        {
            SoundManager.Instance.SFXPlay("기본", basicSound);
        }
    }
}
