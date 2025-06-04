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

    //기본공격 쿨타임
    float baseAttackCoolTime = 4;

    //다른 몬스터가 있는지 없는지 확인하는 변수
    Transform anyMonster = null;

    int baseAtkCount = 0;

    //기본공격 애니메이션 코루틴
    IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(baseAtkAnimationTime);
        animator.SetBool("isAttack", false);
    }

    //기본공격
    IEnumerator BaseAtk()
    {
        while (true)
        {
            if (!isStun)
            {
                StartCoroutine(BaseAttackAnimation());
                if (baseAtkCount < 8)
                {
                    // 쥐 스폰
                    for (int i = 0; i < 3; i++)
                    {
                        Monster mouse = ObjectPoolManager.instance.GetFromPool(mousePrefab);
                        if (mouse != null)
                        {
                            mouse.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                    }

                    // 토끼 스폰
                    for (int i = 0; i < 2; i++)
                    {
                        Monster rabbit = ObjectPoolManager.instance.GetFromPool(rabbitPrefab);
                        if (rabbit != null)
                        {
                            rabbit.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                    }

                    // 원숭이 스폰
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
                    case 0: // 지네
                        Monster centipede = ObjectPoolManager.instance.GetFromPool(centipedePrefab);
                        if (centipede != null)
                        {
                            centipede.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                        }
                        break;

                    case 1: // 사마귀
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



    //매프레임마다 몬스터가 있는지 없는지 확인하는 코루틴
    IEnumerator CheckAnyMonster()
    {
        while (true)
        {
            //플레이어 위치 확인
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

            //몬스터가 존재하는지 확인
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
            //체력이 30미만일때 기본공격속도 2배
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
    //죽었을때 애니메이션 코루틴
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


        //몬스터가 죽었을때
        if (hp <= 0)
        {
            StartCoroutine(DieAnimation());
        }
    }
}