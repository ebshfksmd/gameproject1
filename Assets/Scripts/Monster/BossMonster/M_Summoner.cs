﻿using System.Collections;
using UnityEngine;

public class M_Summoner : Monster
{
    [SerializeField] M_mouse mousePrefab;
    [SerializeField] M_Rabbit rabbitPrefab;
    [SerializeField] M_Monkey monkeyPrefab;
    [SerializeField] M_Centipede centipedePrefab;
    [SerializeField] M_Mantis mantisPrefab;

    [SerializeField] private GameObject Map;

    [SerializeField] private Transform spawnContainer; // 소환된 몬스터들의 부모 오브젝트

    int baseAtkCount = 0;
    bool isDead = false;
    SpriteRenderer spr;


    float tempSkillCoolTime;
    float tempBaseAtkCoolTime;

    private void Start()
    {
        tempSkillCoolTime = skillCoolTime;
        tempBaseAtkCoolTime = baseAtkCoolTime;
        spr = GetComponent<SpriteRenderer>();
        StartCoroutine(WaitForPlayer());
    }

    IEnumerator WaitForPlayer()
    {
        while (GameObject.FindGameObjectWithTag("Player") == null)
            yield return null;

        target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(CheckAnyMonster());
        StartCoroutine(BaseAtk());
        StartCoroutine(BossSkill());
    }

    void Update()
    {
        if (isDead) return;

        if (hpBarInstance != null)
        {
            hpBarInstance.value = hp;
            hpBarInstance.transform.position = transform.position + Vector3.up;
        }

        if (hp <= 0)
            StartCoroutine(DieAnimation());
    }

    IEnumerator BaseAttackAnimation()
    {
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(baseAtkAnimationTime);
        animator.SetBool("isAttack", false);
    }

    IEnumerator BaseAtk()
    {
        while (true)
        {
            if (!isStun)
            {
                StartCoroutine(BaseAttackAnimation());

                if (baseAtkCount < 8)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Monster mob = Instantiate(mousePrefab, GetSpawnPosition(), Quaternion.identity);
                        if (spawnContainer != null) mob.transform.SetParent(spawnContainer);
                        mob.uiCanvas = uiCanvas;
                        mob.InitializeHpBar();
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Monster mob = Instantiate(rabbitPrefab, GetSpawnPosition(), Quaternion.identity);
                        if (spawnContainer != null) mob.transform.SetParent(spawnContainer);
                        mob.uiCanvas = uiCanvas;
                        mob.InitializeHpBar();
                    }

                    {
                        Monster mob = Instantiate(monkeyPrefab, GetSpawnPosition(), Quaternion.identity);
                        if (spawnContainer != null) mob.transform.SetParent(spawnContainer);
                        mob.uiCanvas = uiCanvas;
                        mob.InitializeHpBar();
                    }

                    baseAtkCount++;
                }

                yield return new WaitForSeconds(baseAtkCoolTime);
            }
            else
            {
                yield return null;
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
                int rand = Random.Range(0, 2);

                Monster mob = null;
                switch (rand)
                {
                    case 0:
                        mob = Instantiate(centipedePrefab, GetSpawnPosition(), Quaternion.identity);
                        break;
                    case 1:
                        mob = Instantiate(mantisPrefab, GetSpawnPosition(), Quaternion.identity);
                        break;
                }

                if (mob != null)
                {
                    if (spawnContainer != null) mob.transform.SetParent(spawnContainer);
                    mob.uiCanvas = uiCanvas;
                    mob.InitializeHpBar();
                }

                StartCoroutine(SkillAnimation());
                yield return new WaitForSeconds(skillCoolTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator CheckAnyMonster()
    {
        while (true)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Monster");
            if (found == null)
            {
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
                canAttacked = true;
            }
            else
            {

                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.5f);
                canAttacked = false;
            }

            baseAtkCoolTime = hp < 30 ? tempBaseAtkCoolTime / 2 : tempBaseAtkCoolTime;
            skillCoolTime = hp < 30 ? tempSkillCoolTime / 2 : tempSkillCoolTime;
            moveDirection = transform.position.x > target.position.x ? 1 : -1;

            yield return null;
        }
    }

    IEnumerator DieAnimation()
    {
        speed = 0f;
        animator.SetBool("isDie", true);
        yield return new WaitForSeconds(0.3f);
        Destroy(hpBarInstance?.gameObject);
        hpBarInstance = null;
        isDead = true;
        Destroy(gameObject);
        animator.SetBool("isDie", false);
    }

    Vector3 GetSpawnPosition()
    {
        float x = 0f;

        do
        {
            x = GetRandomX();
        } while ((target.position.x + x < Map.transform.position.x - 50) || (target.position.x + x > Map.transform.position.x + 12));
        return new Vector3(target.position.x + x, 1, 0);
    }

    float GetRandomX()
    {
        return Random.Range(-8, 8);
       
    }

    
}