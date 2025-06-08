using System.Collections;
using UnityEngine;

public class Animal : Monster
{
    [Header("이동 및 공격 범위 설정")]
    public float moveDistance;
    public float targetDistance;
    public float stopDistance;

    [Header("공격 관련 설정")]
    [SerializeField] protected float baseAtkDistance;
    [SerializeField] protected float castingTime;
    [SerializeField] protected int baseAttackPower;

    protected Vector3 startPos;
    protected bool isDead = false;
    protected bool prepareAtk = false;
    protected bool isTracking = false;

    [HideInInspector] public bool inStopDistance = false;
    public int maxHp = 100;

    protected virtual void Start()
    {
        startPos = transform.position;
        moveDirection = (Random.Range(0, 2) == 0) ? 1 : -1;

        StartCoroutine(WaitForTargetThenMove());
    }

    private IEnumerator WaitForTargetThenMove()
    {
        while (target == null)
        {
            yield return null;
        }

        if (moveDistance != 0)
        {
            StartCoroutine(RandomMove());
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;

        UpdateHpBar();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance && !isStun)
        {
            isTracking = true;

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            animator.SetBool("isWalk", true);
            moveDirection = (transform.position.x > target.position.x) ? -1 : 1;
        }
        else if (distanceToTarget > targetDistance && !isStun)
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

        if (distanceToTarget < baseAtkDistance && !prepareAtk && !isStun)
        {
            prepareAtk = true;
            StartCoroutine(AnimalBaseBasicAtk());
        }

        if (hp <= 0 && !isDead)
        {
            StartCoroutine(DieAnimation());
        }
    }

    protected void UpdateHpBar()
    {
        if (hpBarInstance != null)
        {
            hpBarInstance.transform.position = transform.position + Vector3.up;

            float ratio = Mathf.Clamp01((float)hp / maxHp);

            Transform fill = hpBarInstance.transform.Find("Fill");
            if (fill != null)
            {
                Vector3 scale = fill.localScale;
                scale.x = ratio;
                fill.localScale = scale;
            }
        }
    }

    protected IEnumerator DieAnimation()
    {
        isDead = true;
        speed = 0f;
        animator.SetBool("isDie", true);

        if (hpBarInstance != null)
        {
            Destroy(hpBarInstance.gameObject);
            hpBarInstance = null;
        }

        yield return new WaitForSeconds(0.3f);
        Destroy(this.gameObject);
        MonsterDeathWatcher.NotifyMonsterKilled();
        animator.SetBool("isDie", false);
    }

    public IEnumerator RandomMove()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget > targetDistance)
            {
                moveDirection *= -1;
            }

            float waitTime = Mathf.Min(moveDistance / speed, 5f);
            yield return new WaitForSeconds(Random.Range(0f, waitTime));
        }
    }

    public IEnumerator AnimalBaseBasicAtk()
    {
        float tempSpeed = speed;
        speed = 0f;

        yield return new WaitForSeconds(baseAtkCoolTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            StartCoroutine(BaseAttackAnimation());
        }

        speed = tempSpeed;
        prepareAtk = false;
    }

    protected virtual IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);

        yield return new WaitForSeconds(baseAtkAnimationTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            if (PlayerTest.instance != null)
            {
                PlayerTest.instance.GetAttacked(atk, baseAttackPower);
            }
            else
            {
                Debug.LogWarning("[Animal] PlayerTest.instance is null");
            }
        }

        animator.SetBool("isAttack", false);
    }
}
