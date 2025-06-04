using System.Collections;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class M_Summoner : Monster
{


    [SerializeField] M_mouse mousePrefab;
    [SerializeField] M_Rabbit rabbitPrefab;
    [SerializeField] M_Monkey monkeyPrefab;
    [SerializeField] M_Centipede centipedePrefab;
    [SerializeField] M_Mantis mantisPrefab;

    //�⺻���� ��Ÿ��
    float baseAttackCoolTime = 4;

    //�ٸ� ���Ͱ� �ִ��� ������ Ȯ���ϴ� ����
    Transform anyMonster = null;

    int baseAtkCount = 0;

    //�⺻���� �ִϸ��̼� �ڷ�ƾ
    IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(baseAtkAnimationTime);
        animator.SetBool("isAttack", false);
    }

    //�⺻����
    IEnumerator BaseAtk()
    {
        while (true)
        {
            if (!isStun)
            {
                StartCoroutine(BaseAttackAnimation());
                if (baseAtkCount < 8)
                {
                    // �� ����
                    for (int i = 0; i < 3; i++)
                    {
                        Monster mouse = ObjectPoolManager.instance.GetFromPool(mousePrefab);
                        if (mouse != null)
                        {
                            mouse.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                    }

                    // �䳢 ����
                    for (int i = 0; i < 2; i++)
                    {
                        Monster rabbit = ObjectPoolManager.instance.GetFromPool(rabbitPrefab);
                        if (rabbit != null)
                        {
                            rabbit.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                    }

                    // ������ ����
                    Monster mongkey = ObjectPoolManager.instance.GetFromPool(monkeyPrefab);
                    if (mongkey != null)
                    {
                        mongkey.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                    }

                    baseAtkCount++;
                }
                yield return new WaitForSeconds(baseAttackCoolTime);
            }


        }
    }



    IEnumerator SkillAnimation()
    {
        animator.SetBool("isSkill", true);
        yield return new WaitForSeconds(skillAtkAnimationTime);
        animator.SetBool("isSkill", false);
    }


    IEnumerator BossSkill()
    {
        while (true)
        {
            if (!isStun)
            {
                switch (Random.Range(0, 2))
                {
                    case 0: // ����
                        Monster centipede = ObjectPoolManager.instance.GetFromPool(centipedePrefab);
                        if (centipede != null)
                        {
                            centipede.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                        break;

                    case 1: // �縶��
                        Monster mantis = ObjectPoolManager.instance.GetFromPool(mantisPrefab);
                        if (mantis != null)
                        {
                            mantis.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                        break;
                }

                StartCoroutine(SkillAnimation());
                yield return new WaitForSeconds(skillCoolTime);
            }

        }
    }

    SpriteRenderer spr;



    //�������Ӹ��� ���Ͱ� �ִ��� ������ Ȯ���ϴ� �ڷ�ƾ
    IEnumerator CheckAnyMonster()
    {
        while (true)
        {
            //�÷��̾� ��ġ Ȯ��
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

            //���Ͱ� �����ϴ��� Ȯ��
            GameObject found = GameObject.FindGameObjectWithTag("Monster");
            if (found != null)
            {
                anyMonster = found.transform;
            }
            else
            {
                anyMonster = null;
            }

            if (anyMonster == null)
            {
                SpriteRenderer tempSpr = GetComponent<SpriteRenderer>();
                spr.color = new Color(tempSpr.color.r, tempSpr.color.g, tempSpr.color.b, 1f);
                canAtk = false;
            }
            else
            {
                SpriteRenderer tempSpr = GetComponent<SpriteRenderer>();
                spr.color = new Color(tempSpr.color.r, tempSpr.color.g, tempSpr.color.b, 0.5f);
                canAtk = true;
            }
            //ü���� 30�̸��϶� �⺻���ݼӵ� 2��
            if (hp < 30)
            {
                baseAttackCoolTime = 15f;
            }
            else
            {
                baseAttackCoolTime = 30f;
            }
            if (transform.position.x > target.position.x)
            {
                moveDirection = 1;
            }
            else
            {
                moveDirection = -1;
            }
            yield return null;
        }
    }

    private bool isDead = false;
    //�׾����� �ִϸ��̼� �ڷ�ƾ
    IEnumerator DieAnimation()
    {
        speed = 0f;
        animator.SetBool("isDie", true);
        yield return new WaitForSeconds(0.3f);
        Destroy(hpBarInstance.gameObject);
        hpBarInstance = null;
        isDead = true;
        Destroy(this.gameObject);
        animator.SetBool("isDie", false);
    }

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        ObjectPoolManager.instance.Init(mousePrefab, 24, 24);
        ObjectPoolManager.instance.Init(rabbitPrefab, 16, 16);
        ObjectPoolManager.instance.Init(monkeyPrefab, 8, 8);
        ObjectPoolManager.instance.Init(centipedePrefab, 3, 3);
        ObjectPoolManager.instance.Init(mantisPrefab, 3, 3);
        StartCoroutine(CheckAnyMonster());
        StartCoroutine(BaseAtk());
        StartCoroutine(BossSkill());
    }

    private void Update()
    {
        if (isDead) return;
        if (hpBarInstance != null)
        {
            hpBarInstance.value = hp;
            hpBarInstance.gameObject.transform.position = transform.position + Vector3.up;
        }


        //���Ͱ� �׾�����
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }
    }
}