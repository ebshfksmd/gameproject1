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


        //�÷��̾�κ����� �Ÿ�
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        //�����ϴ� ������ �⺻���� �����ȿ� �÷��̾ ���ٸ� ������������
        if (distanceToTarget < baseAtkDistance)
        {
            PlayerTest.instance.GetAttacked(atk, baseAttackPower);
        }
        animator.SetBool("isAttack", false);
    }


    public IEnumerator AnimalBaseBasicAtk()
    {
        
        float tempSpeed = speed;
        speed = 0f;

        yield return new WaitForSeconds(animalBaseAtkCoolTime);

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < baseAtkDistance)
        {
            StartCoroutine(BaseAttackAnimation());
        }

        speed = tempSpeed;
        prepareAtk = false;
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

    [HideInInspector]
    public bool inStopDistance = false;
   private void Update()
{
    if (isDead) return;

    // HP�� ��ġ �� �� ����
    if (hpBarInstance != null)
    {
        hpBarInstance.value = hp;
        hpBarInstance.gameObject.transform.position = transform.position + Vector3.up;
    }

    float distanceToTarget = Vector3.Distance(transform.position, target.position);

    // inStopDistance�� ��Ȯ�� �Ÿ� �������θ� �Ǵ�
    inStopDistance = (distanceToTarget <= stopDistance || distanceToTarget <= baseAtkDistance);

    if (distanceToTarget < targetDistance && distanceToTarget > stopDistance)
    {
        // �÷��̾ ���� ��
        isTracking = true;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        animator.SetBool("isWalk", true);

        // �÷��̾� ��ġ�� ���� moveDirection ����
        moveDirection = (transform.position.x > target.position.x) ? -1 : 1;

        // ���� ���� �� startPos �������� ���� (�̵� �Ÿ� ���� ���� ����)
    }
    else if (distanceToTarget > targetDistance)
    {
        // �÷��̾ ���� ��, �⺻ ���� �̵� �Ǵ� �¿� �̵�
        isTracking = false;

        animator.SetBool("isWalk", true);

        transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;

        float distanceFromStart = transform.position.x - startPos.x;
        if (Mathf.Abs(distanceFromStart) > moveDistance && moveDistance != 0)
        {
            moveDirection *= -1;           // ���� ����
            startPos = transform.position; // ���� �ٲ� �� ���� ��ġ ����
        }
    }
    else
    {
        // ���� ����
        isTracking = false;
        animator.SetBool("isWalk", false);
    }

    // �⺻ ���� ó��
    if (distanceToTarget < baseAtkDistance && !prepareAtk)
    {
        prepareAtk = true;
        StartCoroutine(AnimalBaseBasicAtk());
    }

    // ���� ó��
    if (hp <= 0 && !isDead)
    {
        StartCoroutine(DieAnimation());
    }
}

}
