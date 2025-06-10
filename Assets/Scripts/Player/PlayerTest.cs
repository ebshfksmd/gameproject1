using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTest : MonoBehaviour
{
    public static PlayerTest instance;

    [Header("스탯")]
    public int atk = 10;
    public int def = 1;
    public int hp = 100;

    [Header("체력 UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private int maxHp = 100;

    [Header("색상 효과용 스프라이트")]
    [SerializeField] private SpriteRenderer sprite1;
    [SerializeField] private SpriteRenderer sprite2;

    public enum Status { basic, cantAtk }
    public Status status = Status.basic;
    private PlayerSwitcher playerSwitcher;

    public static readonly List<PlayerTest> AllPlayers = new List<PlayerTest>();

    void OnDestroy()
    {
        AllPlayers.Remove(this);
    }
    private void Awake()
    {
        AllPlayers.Add(this);
        playerSwitcher = Object.FindFirstObjectByType<PlayerSwitcher>();
        instance = this;
        hp = maxHp;
        UpdateHealthUI();
    }

    private void OnEnable()
    {
        instance = this;
        UpdateHealthUI();
    }

    public void SetHealthUI(Slider slider, TextMeshProUGUI text)
    {
        hpSlider = slider;
        hpText = text;
        UpdateHealthUI();
    }

    public void GetAttacked(int dmg, int power)
    {
        ApplyDamage(dmg, power, Color.red);
    }

    public void GetAttacked2(int dmg, int power)
    {
        ApplyDamage(dmg, power, Color.blue);
    }

    private void ApplyDamage(int dmg, int power, Color hitColor)
    {
        float rawDamage = ((float)dmg / (100f + def)) * power;
        int damageAmount = Mathf.Max(1, Mathf.RoundToInt(rawDamage));
        hp = Mathf.Max(0, hp - damageAmount);
        UpdateHealthUI();

        if (gameObject.activeInHierarchy)
            StartCoroutine(HitEffect(hitColor));

        if (hp == 0 && playerSwitcher != null)
        {
            playerSwitcher.OnPlayerDeath(this.gameObject);
        }
    }
    public int MaxHp
    {
        get => maxHp;
        set
        {
            maxHp = Mathf.Max(1, value); // 최소 1 이상 보장
            UpdateHealthUI();
        }
    }
    public void BuffDefense(int amount)
    {
        def += amount;
    }
    public void Heal(int amount)
    {
        if (hp <= 0) return;
        hp = Mathf.Min(hp + amount, maxHp);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = hp;
        }

        if (hpText != null)
        {
            hpText.text = $"HP: {hp} / {maxHp}";
        }
    }

    private IEnumerator HitEffect(Color effectColor)
    {
        if (sprite1 != null) sprite1.color = effectColor;
        if (sprite2 != null) sprite2.color = effectColor;
        yield return new WaitForSeconds(1f);
        if (sprite1 != null) sprite1.color = new Color32(196, 250, 255, 255);
        if (sprite2 != null) sprite2.color = new Color32(196, 250, 255, 255);
    }


}
