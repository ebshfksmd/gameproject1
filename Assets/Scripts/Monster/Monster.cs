using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{


    //���� ������Ʈ
    public GameObject GameObject;


    //ü��
    public int hp;
    //���ݷ�
    public int atk;
    //����
    public int def;
    //�̵��ӵ�
    public float speed;

    private Animator animator;


    public string PoolKey { get; set; }



    protected bool canAtk;
    //����
    //1: ������ -1: ����
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

    //�÷��̾� ����
    [HideInInspector]
    public Transform target;

    [HideInInspector]
    public enum MonsterType
    {
        animal,
        human,
        boss
    }



    [HideInInspector]
    public MonsterType type;


    public IObjectPool<GameObject> Pool { get; set; }



    //���� ���� ��
    public void GetAttacked(int dmg, int power)
    {
        hp -= (dmg / (100 + def))*power;
        Debug.Log($"{gameObject.name} took {(dmg / (100 + def)) * power} damage, HP left: {hp}");
    }






    public virtual void Skill()
    {

    }








    public void Awake()
    {
        animator = GetComponent<Animator>();
        //�÷��̾ Ÿ������ ����
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }


    private Coroutine debuffRoutine;

    public void ApplyAttackDebuff(float percentReduce, float duration)
    {
        // �̹� ����� ���̸� ����
        if (debuffRoutine != null)
            StopCoroutine(debuffRoutine);

        debuffRoutine = StartCoroutine(AttackDebuffCoroutine(percentReduce, duration));
    }

    private IEnumerator AttackDebuffCoroutine(float percentReduce, float duration)
    {
        float original = (float)atk;
        atk = (int)(atk * (1f - percentReduce));

        yield return new WaitForSeconds(duration);

        atk = (int)original;
        debuffRoutine = null;
    }

    public int CurrentHealth => hp;

    private Coroutine moveDebuffRoutine;
    private Coroutine defDebuffRoutine;

    public void ApplyMoveSpeedDebuff(float percentReduce, float duration)
    {
        if (moveDebuffRoutine != null) StopCoroutine(moveDebuffRoutine);
        moveDebuffRoutine = StartCoroutine(MoveDebuffCoroutine(percentReduce, duration));
    }

    private IEnumerator MoveDebuffCoroutine(float percentReduce, float duration)
    {
        float orig = speed;
        speed = speed * (1f - percentReduce);
        yield return new WaitForSeconds(duration);
        speed = orig;
        moveDebuffRoutine = null;
    }

    public void ApplyDefenseDebuff(int amount, float duration)
    {
        if (defDebuffRoutine != null) StopCoroutine(defDebuffRoutine);
        defDebuffRoutine = StartCoroutine(DefenseDebuffCoroutine(amount, duration));
    }

    private IEnumerator DefenseDebuffCoroutine(int amount, float duration)
    {
        int orig = def;
        def = Mathf.Max(0, def - amount);
        yield return new WaitForSeconds(duration);
        def = orig;
        defDebuffRoutine = null;
    }
}