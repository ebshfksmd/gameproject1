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

        // 1) �״� �ִϸ��̼� ���
        anim.Play("die");

        // 2) �ִϸ��̼� ���̸�ŭ ���
        //    (Layer 0, ���� ��� ���� State ���̸� �����ɴϴ�)
        var state = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length);

        // 3) ���� ĳ���ͷ� ��ȯ ��û
        var switcher = FindObjectOfType<PlayerSwitcher>();
        if (switcher != null)
            switcher.SwitchToNextPublic();

        // 4) (�ɼ�) �� ������Ʈ ��Ȱ��ȭ
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
