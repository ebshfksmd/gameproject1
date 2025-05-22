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
    //�⺻���� ��Ÿ��
    float baseAttackCoolTime = 30;
    float skillCoolTime = 60;

    //����
    //1: ���� -1: ������
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


    //�ٸ� ���Ͱ� �ִ��� ������ Ȯ���ϴ� ����
    Transform anyMonster = null;

    int baseAtkCount = 0;
    //�⺻����
    IEnumerator BaseAtk()
    {
        while (true)
        {
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

            yield return new WaitForSeconds(skillCoolTime);
        }
    }

    SpriteRenderer spr;
    bool canAtk = false;


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
                Debug.Log(spr.color.a);
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