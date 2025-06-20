using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    [HideInInspector] public bool IsPooled { get; set; } = false;

    public GameObject GameObject;

    [SerializeField] private Slider hpBar;             // HP 바 프리팹
    [SerializeField] public Canvas uiCanvas;          // HP 바를 붙일 UI 캔버스 (직접 지정)

    public int hp;
    public int atk;
    public int def;
    public float speed;

    protected Animator animator;
    public float baseAtkAnimationTime;
    public float skillAtkAnimationTime;
    [SerializeField] protected float baseAtkCoolTime;
    [SerializeField] protected float skillCoolTime;
    //기절했는지 아닌지
    protected bool isStun = false;
    protected bool canAttacked = true;
    public string PoolKey { get; set; }

    protected bool canAtk;
    int direction = 1;

    public int moveDirection
    {
        get { return direction; }
        set
        {
            if (value == 1)
                transform.rotation = Quaternion.Euler(0, 180, 0);
            else if (value == -1)
                transform.rotation = Quaternion.identity;
            direction = value;
        }
    }

    [HideInInspector] public static Transform target;

    [HideInInspector]
    public enum MonsterType { animal, human, boss }

    [HideInInspector] public MonsterType type;

    public IObjectPool<GameObject> Pool { get; set; }

    private Renderer monsterRenderer;
    private Color originalColor;
    protected Slider hpBarInstance;

    private Coroutine debuffRoutine;
    private Coroutine moveDebuffRoutine;
    private Coroutine defDebuffRoutine;

    public int CurrentHealth => hp;

    public virtual void Awake()
    {
        animator = GetComponent<Animator>();

        if (uiCanvas == null)
        {
            Debug.Log("UI Canvas가 설정되지 않았습니다. Inspector에서 Canvas를 지정하세요.");
        }

        GameObject targetObj = GameObject.FindGameObjectWithTag("Player");
        if (targetObj != null)
        {
            target = targetObj.transform;
            Debug.Log($"[Monster] 타겟 설정 완료: {target.name}");
        }
        else
        {
            Debug.LogWarning("[Monster] Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        monsterRenderer = GetComponentInChildren<Renderer>();
        if (monsterRenderer != null)
        {
            originalColor = monsterRenderer.material.color;
        }
        if (uiCanvas != null)
        {
            InitializeHpBar();
        }
    }

    // 새로 추가한 초기화 함수 (uiCanvas 할당 후 반드시 호출)
    public void InitializeHpBar()
    {
        if (uiCanvas != null && hpBar != null)
        {
            hpBarInstance = Instantiate(hpBar, uiCanvas.transform);
            hpBarInstance.maxValue = hp;
            hpBarInstance.minValue = 0;
            hpBarInstance.value = hp;
        }
        else
        {
            Debug.LogWarning("InitializeHpBar 실패: uiCanvas 또는 hpBar가 할당되지 않았습니다.");
        }
    }

    public void GetAttacked(int dmg, int power)
    {
        if (canAttacked)
        {
            hp -= (dmg / (100 + def)) * power;

            if (hpBarInstance != null)
                hpBarInstance.value = hp;

            StartCoroutine(HitAnimation());

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDir = new Vector2(moveDirection * -1, 1f).normalized;
                rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
            }
        }

    }

    IEnumerator HitAnimation()
    {
        animator.SetBool("isHit", true);
        if (monsterRenderer != null)
        {
            monsterRenderer.material.color = Color.red;
        }

        yield return new WaitForSeconds(1f);

        if (monsterRenderer != null)
        {
            monsterRenderer.material.color = originalColor;
        }

        animator.SetBool("isHit", false);
    }

    public virtual void Skill() { }

    public void ApplyAttackDebuff(float percentReduce, float duration)
    {
        if (debuffRoutine != null)
            StopCoroutine(debuffRoutine);
        debuffRoutine = StartCoroutine(AttackDebuffCoroutine(percentReduce, duration));
    }

    private IEnumerator AttackDebuffCoroutine(float percentReduce, float duration)
    {
        float original = atk;
        atk = (int)(atk * (1f - percentReduce));
        yield return new WaitForSeconds(duration);
        atk = (int)original;
        debuffRoutine = null;
    }

    public void ApplyMoveSpeedDebuff(float percentReduce, float duration)
    {
        if (moveDebuffRoutine != null)
            StopCoroutine(moveDebuffRoutine);
        moveDebuffRoutine = StartCoroutine(MoveDebuffCoroutine(percentReduce, duration));
    }

    private IEnumerator MoveDebuffCoroutine(float percentReduce, float duration)
    {
        float orig = speed;
        speed *= (1f - percentReduce);
        yield return new WaitForSeconds(duration);
        speed = orig;
        moveDebuffRoutine = null;
    }

    public void ApplyDefenseDebuff(int amount, float duration)
    {
        if (defDebuffRoutine != null)
            StopCoroutine(defDebuffRoutine);
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

    public void ApplyStun()
    {
        StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        isStun = true;
        yield return new WaitForSeconds(2f);
        isStun = false;
    }
}