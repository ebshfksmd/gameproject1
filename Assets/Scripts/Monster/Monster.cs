using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;
public class Monster : MonoBehaviour
{
    [HideInInspector]
    public bool IsPooled { get; set; } = false;

    //게임 오프젝트
    public GameObject GameObject;


    [SerializeField] Slider hpBar;

    //체력
    public int hp;
    //공격력
    public int atk;
    //방어력
    public int def;
    //이동속도
    public float speed;

    protected Animator animator;


    //기본공격 애니메이션 시간
    public float baseAtkAnimationTime;
    //스킬 애니메이션 시간
    public float skillAtkAnimationTime;


    public string PoolKey { get; set; }



    protected bool canAtk;
    //방향
    //1: 오른쪽 -1: 왼쪽
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

    //플레이어 추적
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

    IEnumerator HitAnimation()
    {
        animator.SetBool("isHit", true);
        if (monsterRenderer != null)
        {
            monsterRenderer.material.color = Color.red;
        }

        //HitAnimation 길이만큼 딜레이
        yield return new WaitForSeconds(1f);

        if (monsterRenderer != null)
        {
            monsterRenderer.material.color = originalColor;
        }
        animator.SetBool("isHit", false);
    }



    //넉백 파워
    private float knockbackPower = 5f;

    public void GetAttacked(int dmg, int power)
    {
        hp -= (dmg / (100 + def)) * power;
        StartCoroutine(HitAnimation());

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackDir = new Vector2(moveDirection, 1f).normalized;
            rb.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
        }
    }





    public virtual void Skill()
    {

    }




    private Renderer monsterRenderer;
    private Color originalColor;


    protected Slider hpBarInstance;
    private Canvas uiCanvas;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        uiCanvas = Object.FindAnyObjectByType<Canvas>();
        hpBarInstance = Instantiate(hpBar, uiCanvas.transform);
        hpBarInstance.maxValue = hp;
        hpBarInstance.minValue = 0;
        hpBarInstance.value = hp;
        animator = GetComponent<Animator>();
        //플레이어를 타켓으로 설정
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        monsterRenderer = GetComponentInChildren<Renderer>();
        if (monsterRenderer != null)
        {
            originalColor = monsterRenderer.material.color;
        }


        GetAttacked(1, 1);
    }


    private Coroutine debuffRoutine;

    public void ApplyAttackDebuff(float percentReduce, float duration)
    {
        // 이미 디버프 중이면 갱신
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