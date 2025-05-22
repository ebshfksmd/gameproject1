using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private int defense = 0;

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;


    private Animator anim;
    private int originalMaxHealth;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        originalMaxHealth = maxHealth;
        currentHealth = maxHealth;

        // HealthSlider �ʱ�ȭ
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        healthSlider.interactable = false;

        UpdateHealthUI();
    }

    /// <summary>
    /// ������ ó��: ���� �� ���� �켱 �Ҹ� �� ü�� ���
    /// </summary>
    public void TakeDamage(int damage)
    {
        anim.Play("hit");
        int afterDefense = damage / (100 + defense);

        currentHealth = Mathf.Clamp(currentHealth - afterDefense, 0, maxHealth);
        if (currentHealth <= 0)
        {
            anim.Play("die");
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        // ü�� ǥ��
        healthSlider.value = currentHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";

    }

    public void BuffMaxHealth(float percent)
    {
        int added = Mathf.RoundToInt(originalMaxHealth * percent);
        maxHealth = originalMaxHealth + added;

        currentHealth = Mathf.Clamp(currentHealth + added, 0, maxHealth);

        healthSlider.maxValue = maxHealth;
        UpdateHealthUI();
    }

    public void BuffDefense(int amount)
    {
        defense += amount;
    }

    public void Heal(int amount)
    {
        anim.Play("retreat");
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    public int CurrentHealth { get { return currentHealth; } }
}
