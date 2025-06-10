using System.Collections;
using UnityEngine;

public class M_Monkey : Animal
{
    float skillCastingTime = 4f;
    float skillDistance = 3f;
    int skillPower = 8;
    float skillCount = 0;
    bool isSkillCasting = false;
    bool isSKillPrepare = true;

    [Header("사운드 클립")]
    [SerializeField] private AudioClip skillSound;
    [SerializeField] private AudioClip idleLoopSound;

    

    private void PlayIdleSound()
    {
        if (idleLoopSound != null)
            SoundManager.Instance.PlayLoopSound(idleLoopSound);
    }

    private void StopIdleSound()
    {
        if (idleLoopSound != null)
            SoundManager.Instance.StopLoopSound(idleLoopSound);
    }

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        if (skillSound != null)
            SoundManager.Instance.SFXPlay("Skill", skillSound);

        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);
    }

    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float prevSpeed = speed;
        speed = 0f;

        animator.SetBool("isWalk", false);
        StopIdleSound(); // 스킬 시전 중 루프 중단
        yield return new WaitForSeconds(skillCastingTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            PlayerTest.instance.GetAttacked2(atk, skillPower);
        }

        StartCoroutine(SkillAnimation());

        speed = prevSpeed;
        animator.SetBool("isWalk", true);
        PlayIdleSound(); // 다시 루프 시작

        isSKillPrepare = false;
        isSkillCasting = false;
    }

    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!isSKillPrepare)
                skillCount += Time.deltaTime;

            if (skillCount >= skillCoolTime)
            {
                isSKillPrepare = true;
                skillCount = 0;
            }

            if (distanceToTarget < skillDistance && isSKillPrepare && !isSkillCasting && !isStun)
            {
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }

    public override void Skill()
    {
        StartCoroutine(SkillCheck());
    }

#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();
        atk = 18;
        hp = 200;
        def = 30;
        type = MonsterType.animal;

        Skill();
        PlayIdleSound();
    }

    private void OnDestroy()
    {
        StopIdleSound(); // 오브젝트 파괴 시 루프 종료
    }
}
