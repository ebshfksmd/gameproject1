using System.Collections;
using UnityEngine;

public class M_Mantis : Human
{
    float skillCastingTime = 2f;
    int skillPower = 15;
    float skillCount = 0;
    bool isSkillCasting = false;
    bool isSKillPrepare = true;
    float skillDistance = 3f;

    [Header("Sound")]
    public AudioClip dieSound; // ÁÖ±Ã »ç¿îµå (Á×À» ¶§ Àç»ý)

    private bool isSkillCheckRunning = false;

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            PlayerTest.instance.GetAttacked2(atk, skillPower);
        }

        animator.SetBool("isWalk", true);
    }

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

    IEnumerator SkillCheck()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!isSKillPrepare)
            {
                skillCount += Time.deltaTime;
            }

            if (skillCount >= skillCoolTime)
            {
                isSKillPrepare = true;
                skillCount = 0;
            }

            if (distanceToTarget < skillDistance && isSKillPrepare && !isSkillCasting && !isStun)
            {
                isSkillCasting = true;
                StartCoroutine(SkillCast());
            }

            yield return null;
        }
    }

    public override void Skill()
    {
        if (!isSkillCheckRunning)
        {
            isSkillCheckRunning = true;
            StartCoroutine(SkillCheck());
        }
    }
 IEnumerator DieAnimation()
    {
        if (dieSound != null)
        {
            SoundManager.Instance.SFXPlay("Die", dieSound);
        }

        return base.DieAnimation();
    }

#pragma warning disable CS0108 // ¸â¹ö°¡ »ó¼ÓµÈ ¸â¹ö¸¦ ¼û±é´Ï´Ù. new Å°¿öµå°¡ ¾ø½À´Ï´Ù.
#pragma warning disable CS0114
    private void Awake()
#pragma warning restore CS0108
#pragma warning restore CS0114
    {
        base.Awake();
        atk = 150;
        hp = 300;
        def = 150;
        type = MonsterType.human;

        Skill();
    }
}
