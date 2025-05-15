using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    //게임 오프젝트
    public GameObject GameObject;


    //체력
    protected float hp;
    //공격력
    protected float atk;
    //방어력
    protected float def;
    //이동속도
    [SerializeField] float speed;


    private Animator animator;



    //행동방식 반경범위
    public float moveDistance;

    //오브젝트 생성지점
    private Vector3 startPos;


    int direction = 1;
    //1: 오른쪽 -1: 왼쪽
    private int moveDirection
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
    private Transform target;




    public IObjectPool<GameObject> Pool { get; set; }
    public void GetAttacked(float dmg,float power)
    {
        hp -= (dmg/(100+def))*power;
    }
    //몬스터가 플레이어를 따라가고있는지
    private bool isTracking=false;
    
    //랜덤 움직임
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

    public virtual void BasicAttack()
    {

    }



    private void Awake()
    {
        animator = GetComponent<Animator>();    
    }



    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        startPos = transform.position;
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

    //플레이어 추적범위
    public float targetDistance;

    //더이상 따라가지않고 멈추게 되는 거리
    public float stopDistance;

        
    

    private void Update()
    {

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // 플레이어가 추적 범위 안에 들어왔고, 아직 stopDistance 이상 떨어졌다면 따라감
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
        {
            isTracking = true;
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            startPos = transform.position;

            animator.SetTrigger("isWalk");
            
            if(transform.position.x> target.position.x)
            {
                moveDirection = -1;
            }
            else
            {
                moveDirection = 1;
            }



        }
        else if(distanceToTarget <= stopDistance)
        {

        }
        else
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


        
    }

    
}
