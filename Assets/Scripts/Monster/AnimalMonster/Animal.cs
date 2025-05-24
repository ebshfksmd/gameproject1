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


    public IEnumerator AnimalBaseBasicAtk()
    {
        animator.SetBool("isAttack", true);
        float tempSpeed = speed;
        speed = 0f;
        yield return new WaitForSeconds(animalBaseAtkCoolTime);
        speed = tempSpeed;
        animator.SetBool("isAttack", false);
        //플레이어로부터의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //공격하는 시점에 기본공격 범위안에 플레이어가 없다면 공격하지않음
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        prepareAtk = false;
    }


    private void Start()
    {
        type = MonsterType.animal;

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

            //걷기 애니메이션
            animator.SetBool("isWalk", true);

            startPos = transform.position;
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




        //*****************몬스터 기본공격*********************

        //플레이어로부터의 거리
        //기본공격범위 안으로 들어왔을때
        if (distanceToTarget < baseAtkDistance && prepareAtk == false)
        {
            prepareAtk = true;

            StartCoroutine(AnimalBaseBasicAtk());
        }

        //몬스터가 죽었을때
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }

    }
}
