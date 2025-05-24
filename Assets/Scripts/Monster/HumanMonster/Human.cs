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
            if (!isTracking)
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

    public IEnumerator HumanBaseBasicAtk()
    {
        animator.SetBool("isAttack", true);
        float tempSpeed = speed;
        speed = 0f;
        yield return new WaitForSeconds(castingTime);
        speed = tempSpeed;
        animator.SetBool("isAttack", false);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
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
        ObjectPoolManager.instance.ReturnToPool(this);
        animator.SetBool("isDie", false);
    }

    //*******************몬스터 기본공격 관련 변수 , 코루틴*************************
    //기본공격 범위
    [SerializeField] float baseAtkDistance;
    //인간형 몬스터가 몇초마다 공격할지 ( 계속 안에 있어야함 )
    [SerializeField] float humanBaseAtkCoolTime;
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

    private void Update()
    {

        if (isDead) return;

        if (hpBarInstance != null)
        {
            hpBarInstance.value = hp;
            hpBarInstance.gameObject.transform.position = transform.position + Vector3.up;
        }
        //**********************************몬스터 움직임********************************
        //플레이어로 부터의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 플레이어가 추적 범위 안에 들어왔고, 아직 stopDistance 이상 떨어졌다면 따라감
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
        {
            isTracking = true;
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            startPos = transform.position;
            //걷기 애니메이션
            animator.SetBool("isWalk", true);


            if (transform.position.x > target.position.x)
            {
                moveDirection = -1;
            }
            else
            {
                moveDirection = 1;
            }



        }
        else if ((distanceToTarget > stopDistance))
        {
            isTracking = false;
            transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

            //걷기 애니메이션
            animator.SetBool("isWalk", true);

            float distanceFromStart = transform.position.x - startPos.x;

            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1; // 방향 반전
            }
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
        //*******************************************************************************************







        //기본공격 범위안에 들어왔을때
        if (distanceToTarget < baseAtkDistance)
        {
            //캐스팅중이 아닐때 범위안에 얼마만큼 있었는지 카운트
            if (!isCast)
            {
                count += Time.deltaTime;
            }
        }
        else
        {
            //공격범위를 벗어나면 머무는 시간을 다시 잼
            count = 0;
        }

        // 기본공격 범위안에 ~초동안 머물렀을때
        if (count >= humanBaseAtkCoolTime)
        {
            StartCoroutine(HumanBaseBasicAtk());
            isCast = true;
            count = 0;

        }

        //몬스터가 죽었을때
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }

    }
}
