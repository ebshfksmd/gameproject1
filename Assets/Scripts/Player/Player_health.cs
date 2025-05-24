using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Player_health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int defense = 0;



    private Animator anim;
    private int originalMaxHealth;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Start()
    {
        originalMaxHealth = maxHealth;
        currentHealth = maxHealth;

    }

    private bool isDead = false;

    public void TakeDamage(int damage, int power)
    {
        if (isDead) return;

        anim.Play("hit");
        int afterDefense = damage / (100 + defense) * power;
        currentHealth = Mathf.Clamp(currentHealth - afterDefense, 0, maxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAndSwitch());
        }
    }

    private IEnumerator DieAndSwitch()
    {
        isDead = true;

        // 1) 죽는 애니메이션 재생
        anim.Play("die");

        // 2) 애니메이션 길이만큼 대기
        //    (Layer 0, 현재 재생 중인 State 길이를 가져옵니다)
        var state = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);

        // 3) 다음 캐릭터로 전환 요청
        var switcher = FindObjectOfType<PlayerSwitcher>();
        if (switcher != null)
            switcher.SwitchToNextPublic();

        // 4) (옵션) 이 오브젝트 비활성화
        gameObject.SetActive(false);
    }

    public void BuffMaxHealth(float percent)
    {
        int added = Mathf.RoundToInt(originalMaxHealth * percent);
        maxHealth = originalMaxHealth + added;

        currentHealth = Mathf.Clamp(currentHealth + added, 0, maxHealth);

    }

    public void BuffDefense(int amount)
    {
        defense += amount;
    }

    public void Heal(int amount)
    {
        anim.Play("retreat");
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    public bool IsDead => isDead;
}
