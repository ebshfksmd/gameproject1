using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Animal : Monster
{
    //행동방식 반경범위
    public float moveDistance;
    //플레이어 추적범위
    public float targetDistance;
    //더이상 따라가지않고 멈추게 되는 거리
    public float stopDistance;



    //오브젝트 생성지점
    private Vector3 startPos;

    [SerializeField] int baseAttackPower;

    public IEnumerator RandomMove()
    {
        while (true)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget > targetDistance)
            {
                moveDirection *= -1;
            }

            //랜덤이동할때 방향이동 최대치 설정
            if (moveDistance / speed < 5)
            {
                yield return new WaitForSeconds(Random.Range(0f, (moveDistance / speed)));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0f, 5f));
            }
        }
    }

    

    //기본공격 애니메이션 코루틴
    IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(baseAtkAnimationTime);


        //플레이어로부터의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //공격하는 시점에 기본공격 범위안에 플레이어가 없다면 공격하지않음
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        animator.SetBool("isAttack", false);
    }


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



    private void Start()
    {
        startPos = transform.position;
        //몬스터의 랜덤 움직임 방향 초기 설정
        if (Random.Range(0, 2) == 0)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }
        if (moveDistance != 0)
        {
            StartCoroutine(RandomMove());
        }


    }



    private bool isDead = false;
    //죽었을때 애니메이션 코루틴
    IEnumerator DieAnimation()
    {
        isDead = true;
        speed = 0f;
        animator.SetBool("isDie", true);
        Destroy(hpBarInstance.gameObject);
        hpBarInstance = null;

        yield return new WaitForSeconds(0.3f);
        ObjectPoolManager.instance.ReturnToPool(this);
        animator.SetBool("isDie", false);
    }

    //*******************몬스터 기본공격 관련 변수 , 코루틴*************************
    //기본공격 범위
    [SerializeField] float baseAtkDistance;
    //동물형 몬스터가몇초마다 기본공격을 할지
    [SerializeField] float animalBaseAtkCoolTime;
    //시전시간 ( 공격할 시점에서 범위 안에있으면됨 )
    [SerializeField] float castingTime;




    //공격 준비중인지
    //animal type 몬스터에서 쓰는 변수
    private bool prepareAtk = false;

    //몬스터가 플레이어를 따라가고있는지
    private bool isTracking = false;

    [HideInInspector]
    public bool inStopDistance = false;
   private void Update()
{
    if (isDead) return;

    // HP바 위치 및 값 갱신
    if (hpBarInstance != null)
    {
        hpBarInstance.value = hp;
        hpBarInstance.gameObject.transform.position = transform.position + Vector3.up;
    }

    float distanceToTarget = Vector3.Distance(transform.position, target.position);

    // inStopDistance를 명확히 거리 기준으로만 판단
    inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

    if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
    {
        // 플레이어를 추적 중
        isTracking = true;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        animator.SetBool("isWalk", true);

        // 플레이어 위치에 따라 moveDirection 설정
        moveDirection = (transform.position.x > target.position.x) ? -1 : 1;

        // 추적 중일 땐 startPos 갱신하지 않음 (이동 거리 기준 유지 위해)
    }
    else if (distanceToTarget > targetDistance)
    {
        // 플레이어가 범위 밖, 기본 랜덤 이동 또는 좌우 이동
        isTracking = false;

        animator.SetBool("isWalk", true);

        transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

        float distanceFromStart = transform.position.x - startPos.x;
        if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
        {
            moveDirection *= -1;           // 방향 반전
            startPos = transform.position; // 방향 바뀔 때 기준 위치 갱신
        }
    }
    else
    {
        // 멈춤 상태
        isTracking = false;
        animator.SetBool("isWalk", false);
    }

    // 기본 공격 처리
    if (distanceToTarget < baseAtkDistance && !prepareAtk)
    {
        prepareAtk = true;
        StartCoroutine(AnimalBaseBasicAtk());
    }

    // 죽음 처리
    if (hp <= 0 && !isDead)
    {
        StartCoroutine(DieAnimation());
    }
}

}
