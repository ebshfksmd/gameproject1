using System.Collections;
using UnityEngine;

public class Human : Monster
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
        animator.SetBool("isAttack", false);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
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
        speed = 0f;
        animator.SetBool("isDie", true);
        Destroy(hpBarInstance.gameObject);
        hpBarInstance = null;
        isDead = true;
        yield return new WaitForSeconds(0.3f);
        Destroy(this.gameObject);
        animator.SetBool("isDie", false);
    }

    //*******************몬스터 기본공격 관련 변수 , 코루틴*************************
    //기본공격 범위
    [SerializeField] float baseAtkDistance;

    //시전시간 ( 공격할 시점에서 범위 안에있으면됨 )
    [SerializeField] float castingTime;

    //범위안에 얼마만큼 머물러있었는지 확인하는 변수
    //human type 몬스터에서 쓰는 변수
    float count = 0;

    //공격시전이 시작되었는지 확인하는 변수
    //human type 몬스터에서 쓰는 변수
    bool isCast = false;



    //몬스터가 플레이어를 따라가고있는지
    private bool isTracking = false;

    [HideInInspector]
    public bool inStopDistance = false;
    private void Update()
    {
        if (isDead) return;

        if (hpBarInstance != null)
        {
            hpBarInstance.value = hp;
            hpBarInstance.gameObject.transform.position = transform.position + Vector3.up;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // inStopDistance 플래그를 거리 기준으로 명확하게 설정
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        // isTracking과 이동 상태 분기
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance && !isStun)
        {
            isTracking = true;
            // 플레이어를 따라감
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            animator.SetBool("isWalk", true);

            // 플레이어 위치에 따라 moveDirection 설정 (추적 시)
            moveDirection = (transform.position.x > target.position.x) ? -1 : 1;

            // 추적 중에는 startPos 갱신하지 않음
        }
        else if (distanceToTarget > targetDistance && !isStun)
        {
            // 플레이어가 범위 밖 -> 기본 이동
            isTracking = false;
            animator.SetBool("isWalk", true);

            transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

            // 이동 거리 계산 (startPos는 Start() 또는 방향 전환 시 갱신)
            float distanceFromStart = transform.position.x - startPos.x;
            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1;           // 방향 반전
                startPos = transform.position; // 방향 전환 시점에 기준 위치 갱신
            }
        }
        else
        {
            // 멈춤 상태
            isTracking = false;
            animator.SetBool("isWalk", false);
        }

        // 공격 쿨다운 관련 처리
        if (distanceToTarget < baseAtkDistance)
        {
            if (!isCast)
            {
                count += Time.deltaTime;
            }
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

        // 죽음 처리
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }
    }

}
