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
        animator.SetBool("isAttack", true);
        float tempSpeed = speed;
        speed = 0f;
        yield return new WaitForSeconds(animalBaseAtkCoolTime);
        speed = tempSpeed;
        animator.SetBool("isAttack", false);
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


    private bool isDead = false;
    //�׾����� �ִϸ��̼� �ڷ�ƾ
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
        if (isDead) return;

        if (hpBarInstance != null)
        {
            hpBarInstance.value = hp;
            hpBarInstance.gameObject.transform.position = transform.position + Vector3.up;
        }
        //**********************************���� ������********************************
        //�÷��̾�� ������ �Ÿ�
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // �÷��̾ ���� ���� �ȿ� ���԰�, ���� stopDistance �̻� �������ٸ� ����
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
        {
            isTracking = true;
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            //�ȱ� �ִϸ��̼�
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

            //�ȱ� �ִϸ��̼�
            animator.SetBool("isWalk", true);

            float distanceFromStart = transform.position.x - startPos.x;

            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1; // ���� ����
            }
        }
        else
        {
            animator.SetBool("isWalk", false);
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

        //���Ͱ� �׾�����
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }

    }
}
