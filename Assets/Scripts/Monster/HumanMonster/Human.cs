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
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget > targetDistance)
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


    //�⺻���� �ִϸ��̼� �ڷ�ƾ
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
        Destroy(this.gameObject);
        animator.SetBool("isDie", false);
    }

    //*******************���� �⺻���� ���� ���� , �ڷ�ƾ*************************
    //�⺻���� ����
    [SerializeField] float baseAtkDistance;

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

        // inStopDistance �÷��׸� �Ÿ� �������� ��Ȯ�ϰ� ����
        inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

        // isTracking�� �̵� ���� �б�
        if (distanceToTarget < targetDistance && distanceToTarget > stopDistance && !isStun)
        {
            isTracking = true;
            // �÷��̾ ����
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            animator.SetBool("isWalk", true);

            // �÷��̾� ��ġ�� ���� moveDirection ���� (���� ��)
            moveDirection = (transform.position.x > target.position.x) ? -1 : 1;

            // ���� �߿��� startPos �������� ����
        }
        else if (distanceToTarget > targetDistance && !isStun)
        {
            // �÷��̾ ���� �� -> �⺻ �̵�
            isTracking = false;
            animator.SetBool("isWalk", true);

            transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

            // �̵� �Ÿ� ��� (startPos�� Start() �Ǵ� ���� ��ȯ �� ����)
            float distanceFromStart = transform.position.x - startPos.x;
            if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
            {
                moveDirection *= -1;           // ���� ����
                startPos = transform.position; // ���� ��ȯ ������ ���� ��ġ ����
            }
        }
        else
        {
            // ���� ����
            isTracking = false;
            animator.SetBool("isWalk", false);
        }

        // ���� ��ٿ� ���� ó��
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

        // ���� ó��
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }
    }

}
