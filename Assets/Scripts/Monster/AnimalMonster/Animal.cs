using System.Collections;
using UnityEngine;

public class Animal : Monster
{
    [Header("이동 및 공격 범위 설정")]
    public float moveDistance;     // 랜덤 이동 반경
    public float targetDistance;   // 추적 시작 범위
    public float stopDistance;     // 추적 멈춤 거리

    [Header("공격 관련 설정")]
    [SerializeField] float baseAtkDistance;          // 공격 사정거리
    [SerializeField] float animalBaseAtkCoolTime;    // 공격 쿨타임
    [SerializeField] float castingTime;              // 캐스팅 시간
    [SerializeField] int baseAttackPower;            // 실제 가해지는 공격력

    private Vector3 startPos;        // 초기 위치 (이동 반경 기준점)
    private bool isDead = false;     // 사망 여부
    private bool prepareAtk = false; // 공격 준비 상태
    private bool isTracking = false; // 추적 중 여부
    [HideInInspector]
    public bool inStopDistance = false;
    public int maxHp = 100;
    private void Start()
    {
        startPos = transform.position;

        // 좌우 무작위 방향 설정
        moveDirection = (Random.Range(0, 2) == 0) ? 1 : -1;

        // 랜덤 이동 시작
        if (moveDistance != 0)
        {
            StartCoroutine(RandomMove());
        }
    }

    public void Update()
    {
        if (isDead) return;

        if (hpBarInstance != null)
        {
            // 1) 위치 조정
            hpBarInstance.transform.position = transform.position + Vector3.up;

            // 2) 체력 비율 계산 (0~1)
            float ratio = Mathf.Clamp01((float)hp / maxHp);

            // 3) Fill 오브젝트 찾기
            Transform fill = hpBarInstance.transform.Find("Fill");
            if (fill != null)
            {
                Vector3 scale = fill.localScale;
                scale.x = ratio; // 좌우 크기 조정
                fill.localScale = scale;
            }
        }


        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
        {
            // 추적 로직
            isTracking = true;

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            animator.SetBool("isWalk", true);
            moveDirection = (transform.position.x > target.position.x) ? -1 : 1;
        }
        else if (distanceToTarget > targetDistance)
        {
            // 랜덤 이동
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
            // 정지 상태
            isTracking = false;
            animator.SetBool("isWalk", false);
        }

        // 공격 시도
        if (distanceToTarget < baseAtkDistance && !prepareAtk)
        {
            prepareAtk = true;
            StartCoroutine(AnimalBaseBasicAtk());
        }

        // 죽음 체크
        if (hp <= 0 && !isDead)
        {
            StartCoroutine(DieAnimation());
        }
    }

    // 몬스터가 죽었을 때 호출
    IEnumerator DieAnimation()
    {
        isDead = true;
        speed = 0f;
        animator.SetBool("isDie", true);

        // HP 바 제거
        if (hpBarInstance != null)
        {
            Destroy(hpBarInstance.gameObject);
            hpBarInstance = null;
        }

        yield return new WaitForSeconds(0.3f);

        // 오브젝트 풀로 반환
        //ObjectPoolManager.instance.ReturnToPool(this);
        Destroy(this.gameObject);

        animator.SetBool("isDie", false);
    }

    // 랜덤 이동 루틴
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

    // 기본 공격 실행 루틴
    public IEnumerator AnimalBaseBasicAtk()
    {
        float tempSpeed = speed;
        speed = 0f;

        yield return new WaitForSeconds(animalBaseAtkCoolTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            StartCoroutine(BaseAttackAnimation());
        }

        speed = tempSpeed;
        prepareAtk = false;
    }

    // 실제 공격 애니메이션과 데미지 적용
    IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(baseAtkAnimationTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            // null 체크 추가: PlayerTest.instance가 없으면 오류 방지
            if (PlayerTest.instance != null)
            {
                PlayerTest.instance.GetAttacked(atk, baseAttackPower);
            }
            else
            {
                Debug.LogWarning("PlayerTest.instance is null. Cannot apply attack.");
            }
        }

        animator.SetBool("isAttack", false);
    }
}
