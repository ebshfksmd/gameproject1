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
        speed = 0f;
        animator.SetBool("isDie", true);
        Destroy(hpBarInstance.gameObject);
        hpBarInstance = null;
        isDead = true;
        yield return new WaitForSeconds(0.3f);
        ObjectPoolManager.instance.ReturnToPool(this);
        animator.SetBool("isDie", false);
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
            startPos = transform.position;
            //�ȱ� �ִϸ��̼�
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

        //���Ͱ� �׾�����
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }

    }
}
