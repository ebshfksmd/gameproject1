using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{


    //게임 오프젝트
    public GameObject GameObject;


    //체력
    protected int hp;
    //공격력
    protected int atk;
    //방어력
    protected int def;

    public int baseAttackPower; 


    //이동속도
    public float speed;



    private Animator animator;

    public string PoolKey { get; set; }

    //행동방식 반경범위
    public float moveDistance;

    //오브젝트 생성지점
    private Vector3 startPos;

    //플레이어 추적범위
    public float targetDistance;

    //더이상 따라가지않고 멈추게 되는 거리
    public float stopDistance;



    //방향
    //1: 오른쪽 -1: 왼쪽
    int direction = 1;
    public int moveDirection
    {
        get
        {
            return direction;
        }
        set
        {
            if(value == 1)
            {
                gameObject.transform.rotation=new Quaternion(0,180,0,0);
            }
            else if(value == -1)
            {
                gameObject.transform.rotation=new Quaternion(0,0,0,0);
            }
            direction = value;
        }
    }

    //플레이어 추적
    [HideInInspector]
    public Transform target;

    [HideInInspector]
    public enum MonsterType
    {
        animal,
        human,
        boss
    }



    [HideInInspector]
    public MonsterType type;


    public IObjectPool<GameObject> Pool { get; set; }



    //공격 받을 때
    public void GetAttacked(int dmg,int power)
    {
        hp -= (dmg/(100+def))*power;
        Debug.Log($"{gameObject.name} took {(dmg / (100 + def)) * power} damage, HP left: {hp}");
    }



    //몬스터가 플레이어를 따라가고있는지
    private bool isTracking=false;
    


    //랜덤 움직임 코루틴
    public IEnumerator RandomMove()
    {
        while(true)
        {
            if(!isTracking)
            {
                moveDirection *= -1;
            }

            //랜덤이동할때 방향이동 최대치 설정
            if(moveDistance / speed < 5)
            {
                yield return new WaitForSeconds(Random.Range(0f,(moveDistance/speed)));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0f,5f));
            }
        }
    }



    public virtual void Skill()
    {

    }








    public void Awake()
    {
        animator = GetComponent<Animator>();    
        //플레이어를 타켓으로 설정
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }



    private void Start()
    {
        startPos = transform.position;
        //몬스터의 랜덤 움직임 방향 초기 설정
        if(Random.Range(0,2)==0)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;   
        }
        if(moveDistance!=0)
        {
            StartCoroutine(RandomMove());
        }
        
    }


    //*******************몬스터 기본공격 관련 변수 , 코루틴*************************
    //기본공격 범위
    [SerializeField] float baseAtkDistance;
    //동물형 몬스터가몇초마다 기본공격을 할지
    [SerializeField] float animalBaseAtkCoolTime;
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

    //공격 준비중인지
    //animal type 몬스터에서 쓰는 변수
    private bool prepareAtk = false;

    public IEnumerator HumanBaseBasicAtk()
    {
        yield return new WaitForSeconds(castingTime);
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        count = 0;
        isCast = false;
    }

    public IEnumerator AnimalBaseBasicAtk()
    {
        yield return new WaitForSeconds(animalBaseAtkCoolTime);
        //플레이어로부터의 거리
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //공격하는 시점에 기본공격 범위안에 플레이어가 없다면 공격하지않음
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        prepareAtk = false;
    }



    //*********************************************************************


    private void Update()
    {


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

            //animator.SetTrigger("isWalk");
            
            if(transform.position.x> target.position.x)
            {
                moveDirection = -1;
            }
            else
            {
                moveDirection = 1;
            }



        }
        else if((distanceToTarget > stopDistance))
        {
            isTracking=false;
            transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;
            //animator.SetTrigger("isWalk");

            float distanceFromStart = transform.position.x - startPos.x;

            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance!=0)
            {
                moveDirection *= -1; // 방향 반전
            }
        }
        //*******************************************************************************************




        //*****************몬스터 기본공격*********************
        if(type==MonsterType.animal)
        {
            //플레이어로부터의 거리
            //기본공격범위 안으로 들어왔을때
            if (distanceToTarget < baseAtkDistance && prepareAtk == false)
            {
                prepareAtk = true;
                
                StartCoroutine(AnimalBaseBasicAtk());
            }
        }
        else if(type==MonsterType.human)
        {

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
        }
        else if(type==MonsterType.boss)
        {

        }
        //*****************************************************











    }
}
