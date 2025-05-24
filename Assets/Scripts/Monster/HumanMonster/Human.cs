using System.Collections;
using UnityEngine;

public class Human : Monster
{
    //�ൿ��� �ݰ����
    public float moveDistance;
    //�÷��̾� ��������
    public float targetDistance;
    //���̻� �������ʰ� ���߰� �Ǵ� �Ÿ�
    public float stopDistance;



    //������Ʈ ��������
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

            //�����̵��Ҷ� �����̵� �ִ�ġ ����
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
        yield return new WaitForSeconds(castingTime);
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
        //������ ���� ������ ���� �ʱ� ����
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



    //*******************���� �⺻���� ���� ���� , �ڷ�ƾ*************************
    //�⺻���� ����
    [SerializeField] float baseAtkDistance;
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



    //���Ͱ� �÷��̾ ���󰡰��ִ���
    private bool isTracking = false;

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
            //animator.SetTrigger("isWalk");

            float distanceFromStart = transform.position.x - startPos.x;

            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1; // ���� ����
            }
        }
        //*******************************************************************************************







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


        //*****************************************************
    }
}
