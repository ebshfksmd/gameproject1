using System.Collections;
using UnityEngine;

public class Human : Monster
{
    [Header("이동 및 추적 설정")]
    public float moveDistance;
    public float targetDistance;
    public float stopDistance;

    [Header("공격 설정")]
    [SerializeField] private int baseAttackPower;
    [SerializeField] private float baseAtkDistance;
    [SerializeField] private float castingTime;

    private Vector3 startPos;
    private float count = 0f;
    private bool isCast = false;
    private bool isTracking = false;
    private bool isDead = false;
    [HideInInspector] public bool inStopDistance = false;

    private void Start()
    {
        startPos = transform.position;
        moveDirection = (Random.Range(0, 2) == 0) ? 1 : -1;

        if (moveDistance != 0)
            StartCoroutine(RandomMove());
    }

    private void Update()
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

        if (distanceToTarget < baseAtkDistance)
        {
            if (!isCast)
                count += Time.deltaTime;
        }
        else
        {
            count = 0;
        }

        if (count >= baseAtkCoolTime && !isStun)
        {
            StartCoroutine(HumanBaseBasicAtk());
            isCast = true;
            count = 0;
        }

        if (hp <= 0 && !isDead)
        {
            StartCoroutine(DieAnimation());
        }
    }

    private void UpdateHpBar()
    {
        if (hpBarInstance != null)
        {
            hpBarInstance.value = hp;
            hpBarInstance.transform.position = transform.position + Vector3.up;
        }
    }

    private IEnumerator DieAnimation()
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

    public IEnumerator HumanBaseBasicAtk()
    {
        float tempSpeed = speed;
        speed = 0f;
        yield return new WaitForSeconds(castingTime);

        StartCoroutine(BaseAttackAnimation());

        speed = tempSpeed;
        count = 0;
        isCast = false;
    }

    private IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(baseAtkAnimationTime);
        animator.SetBool("isAttack", false);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            if (PlayerTest.instance != null)
            {
                PlayerTest.instance.GetAttacked(atk, baseAttackPower);
            }
            else
            {
                Debug.LogWarning("[Human] PlayerTest.instance is null");
            }
        }
    }
}
