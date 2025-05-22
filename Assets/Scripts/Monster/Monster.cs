using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{


    //���� ������Ʈ
    public GameObject GameObject;


    //ü��
    protected int hp;
    //���ݷ�
    protected int atk;
    //����
    protected int def;

    public int baseAttackPower; 


    //�̵��ӵ�
    public float speed;



    private Animator animator;

    public string PoolKey { get; set; }

    //�ൿ��� �ݰ����
    public float moveDistance;

    //������Ʈ ��������
    private Vector3 startPos;

    //�÷��̾� ��������
    public float targetDistance;

    //���̻� �������ʰ� ���߰� �Ǵ� �Ÿ�
    public float stopDistance;



    //����
    //1: ������ -1: ����
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

    //�÷��̾� ����
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



    //���� ���� ��
    public void GetAttacked(int dmg,int power)
    {
        hp -= (dmg/(100+def))*power;
        Debug.Log($"{gameObject.name} took {(dmg / (100 + def)) * power} damage, HP left: {hp}");
    }



    //���Ͱ� �÷��̾ ���󰡰��ִ���
    private bool isTracking=false;
    


    //���� ������ �ڷ�ƾ
    public IEnumerator RandomMove()
    {
        while(true)
        {
            if(!isTracking)
            {
                moveDirection *= -1;
            }

            //�����̵��Ҷ� �����̵� �ִ�ġ ����
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
        //�÷��̾ Ÿ������ ����
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }



    private void Start()
    {
        startPos = transform.position;
        //������ ���� ������ ���� �ʱ� ����
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


    //*******************���� �⺻���� ���� ���� , �ڷ�ƾ*************************
    //�⺻���� ����
    [SerializeField] float baseAtkDistance;
    //������ ���Ͱ����ʸ��� �⺻������ ����
    [SerializeField] float animalBaseAtkCoolTime;
    //�ΰ��� ���Ͱ� ���ʸ��� �������� ( ��� �ȿ� �־���� )
    [SerializeField] float humanBaseAtkCoolTime;
    //�����ð� ( ������ �������� ���� �ȿ�������� )
    [SerializeField] float castingTime;

    //�����ȿ� �󸶸�ŭ �ӹ����־����� Ȯ���ϴ� ����
    //human type ���Ϳ��� ���� ����
    float count = 0;

    //���ݽ����� ���۵Ǿ����� Ȯ���ϴ� ����
    //human type ���Ϳ��� ���� ����
    bool isCast = false;

    //���� �غ�������
    //animal type ���Ϳ��� ���� ����
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
        //�÷��̾�κ����� �Ÿ�
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //�����ϴ� ������ �⺻���� �����ȿ� �÷��̾ ���ٸ� ������������
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        prepareAtk = false;
    }



    //*********************************************************************


    private void Update()
    {


        //**********************************���� ������********************************
        //�÷��̾�� ������ �Ÿ�
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // �÷��̾ ���� ���� �ȿ� ���԰�, ���� stopDistance �̻� �������ٸ� ����
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
                moveDirection *= -1; // ���� ����
            }
        }
        //*******************************************************************************************




        //*****************���� �⺻����*********************
        if(type==MonsterType.animal)
        {
            //�÷��̾�κ����� �Ÿ�
            //�⺻���ݹ��� ������ ��������
            if (distanceToTarget < baseAtkDistance && prepareAtk == false)
            {
                prepareAtk = true;
                
                StartCoroutine(AnimalBaseBasicAtk());
            }
        }
        else if(type==MonsterType.human)
        {

            //�⺻���� �����ȿ� ��������
            if (distanceToTarget < baseAtkDistance)
            {
                //ĳ�������� �ƴҶ� �����ȿ� �󸶸�ŭ �־����� ī��Ʈ
                if (!isCast)
                {
                    count += Time.deltaTime;
                }
            }
            else
            {
                //���ݹ����� ����� �ӹ��� �ð��� �ٽ� ��
                count = 0;
            }

            // �⺻���� �����ȿ� ~�ʵ��� �ӹ�������
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
