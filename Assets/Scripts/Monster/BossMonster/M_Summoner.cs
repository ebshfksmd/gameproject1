using System.Collections;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class M_Summoner : MonoBehaviour
{
    [HideInInspector]
    public int hp = 120;
    [HideInInspector]
    public int atk = 150;
    [HideInInspector]
    public int def = 100;
    [SerializeField] M_mouse mousePrefab;
    [SerializeField] M_Rabbit rabbitPrefab;
    [SerializeField] M_Mongkey mongkeyPrefab;
    [SerializeField] M_Centipede centipedePrefab;
    [SerializeField] M_Mantis mantisPrefab;
    Transform target;
    //기본공격 쿨타임
    float baseAttackCoolTime = 30;
    float skillCoolTime = 60;

    //방향
    //1: 왼쪽 -1: 오른쪽
    int direction = 1;
    public int moveDirection
    {
        get
        {
            return direction;
        }
        set
        {
            if (value == 1)
            {
                gameObject.transform.rotation = new Quaternion(0, 180, 0, 0);
            }
            else if (value == -1)
            {
                gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            direction = value;
        }
    }


    //다른 몬스터가 있는지 없는지 확인하는 변수
    Transform anyMonster = null;

    int baseAtkCount = 0;
    //기본공격
    IEnumerator BaseAtk()
    {
        while (true)
        {
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
                Monster mongkey = ObjectPoolManager.instance.GetFromPool(mongkeyPrefab);
                if (mongkey != null)
                {
                    mongkey.transform.position = new Vector3(target.position.x + Random.Range(-8, 8), 1, 0);
                }

                baseAtkCount++;
            }

            yield return new WaitForSeconds(baseAttackCoolTime);
        }
    }


    IEnumerator Skill()
    {
        while (true)
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

            yield return new WaitForSeconds(skillCoolTime);
        }
    }

    SpriteRenderer spr;
    bool canAtk = false;


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
                Debug.Log(spr.color.a);
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
    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        ObjectPoolManager.instance.Init(mousePrefab, 24, 24);
        ObjectPoolManager.instance.Init(rabbitPrefab, 16, 16);
        ObjectPoolManager.instance.Init(mongkeyPrefab, 8, 8);
        ObjectPoolManager.instance.Init(centipedePrefab, 3, 3);
        ObjectPoolManager.instance.Init(mantisPrefab, 3, 3);
        StartCoroutine(CheckAnyMonster());
        StartCoroutine(BaseAtk());
        StartCoroutine(Skill());
    }
}