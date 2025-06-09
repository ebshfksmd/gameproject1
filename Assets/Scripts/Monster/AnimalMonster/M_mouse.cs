using System.Collections;
using UnityEngine;

public class M_mouse : Animal
{
    float skillCastingTime = 3f;
    float skillDistance = 3f;
    int skillPower = 4;

    float skillCount = 0f;
    bool isSkillCasting = false;
    bool isSkillPrepared = true;

    [Header("Sound")]
    [SerializeField] private AudioClip crySFX;
    [SerializeField] private AudioClip baseAtkSFX;
    [SerializeField] private AudioClip skillSFX;

    private bool hasPlayedCrySound = false;
    private AudioSource sfxSource;

    public override void Awake()
    {
        base.Awake();

        hp = 100;
        atk = 150;
        def = 0;
        moveDistance = 5000f;
        type = MonsterType.animal;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = 1f;
    }

    protected override void Update()
    {
        if (isDead) return;

        UpdateHpBar();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        HandleMovement(distanceToTarget);

        if (!isSkillPrepared)
        {
            skillCount += Time.deltaTime;
            if (skillCount >= skillCoolTime)
            {
                isSkillPrepared = true;
                skillCount = 0f;
                Debug.Log("[쥐] 스킬 준비 완료");
            }
        }

        if (distanceToTarget < skillDistance && isSkillPrepared && !isSkillCasting && !isStun)
        {
            Debug.Log("[쥐] 스킬 시전 시작");
            StartCoroutine(SkillCast());
            return;
        }

        if (distanceToTarget < baseAtkDistance && !prepareAtk && !isSkillCasting && !isStun)
        {
            prepareAtk = true;
            StartCoroutine(AnimalBaseBasicAtk());
        }

        if (hp <= 0 && !isDead)
        {
            StartCoroutine(DieAnimation());
        }
    }

    void HandleMovement(float distanceToTarget)
    {
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
        {
            if (!hasPlayedCrySound && crySFX != null)
            {
                hasPlayedCrySound = true;
                SoundManager.Instance.SFXPlay("MouseCry", crySFX);
                Debug.Log("[M_mouse] 울음소리 SFX 재생됨");
            }

            isTracking = true;
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            animator.SetBool("isWalk", true);
            moveDirection = (transform.position.x > target.position.x) ? -1 : 1;
        }
        else if (distanceToTarget > targetDistance)
        {
            isTracking = false;
            animator.SetBool("isWalk", true);
            transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

            float distanceFromStart = transform.position.x - startPos.x;
            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1;
                startPos = transform.position;
            }
        }
        else
        {
            isTracking = false;
            animator.SetBool("isWalk", false);
        }
    }

    IEnumerator AnimalBaseBasicAtk()
    {
        animator.SetBool("isAttack", true);

        if (baseAtkSFX != null)
            sfxSource.PlayOneShot(baseAtkSFX);

        yield return new WaitForSeconds(baseAtkAnimationTime);

        if (Vector3.Distance(transform.position, target.position) <= baseAtkDistance)
        {
            if (PlayerTest.instance != null)
            {
                Debug.Log($"[기본공격] {gameObject.name}이(가) {atk} 피해를 입힘");
                PlayerTest.instance.GetAttacked2(atk, skillPower);
            }
        }

        sfxSource.Stop();
        animator.SetBool("isAttack", false);
        yield return new WaitForSeconds(baseAtkCoolTime);
        prepareAtk = false;
    }

    IEnumerator SkillCast()
    {
        isSkillCasting = true;
        float prevSpeed = speed;
        speed = 0f;

        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(skillCastingTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            StartCoroutine(SkillAnimation());
        }

        speed = prevSpeed;
        animator.SetBool("isWalk", true);

        isSkillPrepared = false;
        isSkillCasting = false;
    }

    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);

        if (skillSFX != null)
            sfxSource.PlayOneShot(skillSFX);

        yield return new WaitForSeconds(skillAtkAnimationTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < skillDistance)
        {
            if (PlayerTest.instance != null)
            {
                Debug.Log($"[스킬공격] {gameObject.name}이(가) {skillPower} 피해를 입힘");
                PlayerTest.instance.GetAttacked2(atk, skillPower);
            }
            else
            {
                Debug.LogWarning("[M_mouse] PlayerTest.instance is null");
            }
        }

        sfxSource.Stop();
        animator.SetBool("isSkill", false);
    }
}
