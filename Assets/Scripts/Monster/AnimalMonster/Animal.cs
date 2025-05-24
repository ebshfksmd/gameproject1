using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Animal : Monster
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


    private void Start()
    {
        type = MonsterType.animal;

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
    //������ ���Ͱ����ʸ��� �⺻������ ����
    [SerializeField] float animalBaseAtkCoolTime;
    //�����ð� ( ������ �������� ���� �ȿ�������� )
    [SerializeField] float castingTime;




    //���� �غ�������
    //animal type ���Ϳ��� ���� ����
    private bool prepareAtk = false;

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




        //*****************���� �⺻����*********************

        //�÷��̾�κ����� �Ÿ�
        //�⺻���ݹ��� ������ ��������
        if (distanceToTarget < baseAtkDistance && prepareAtk == false)
        {
            prepareAtk = true;

            StartCoroutine(AnimalBaseBasicAtk());
        }


        //*****************************************************

    }
}