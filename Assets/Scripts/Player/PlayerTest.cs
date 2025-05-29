using System.Collections;
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

    public enum Status
    {
        basic,
        cantAtk
    }

    public Status status = Status.basic;

    private void Awake()
    {
        instance = this;
        hp = maxHp;
        UpdateHealthUI();
        Debug.Log($"[PlayerTest] 등록됨: {gameObject.name}");
    }

    private void OnEnable()
    {
        instance = this;
        UpdateHealthUI();
    }

    public void GetAttacked(int dmg, int power)
    {
        Debug.Log("맞음");
        ApplyDamage(dmg, power, Color.red);
    }

    public void GetAttacked2(int dmg, int power)
    {
        Debug.Log("쳐맞음");
        ApplyDamage(dmg, power, Color.blue);
    }

    private void ApplyDamage(int dmg, int power, Color hitColor)
    {
        float rawDamage = ((float)dmg / (100f + def)) * power;
        int damageAmount = Mathf.Max(1, Mathf.RoundToInt(rawDamage));

        hp -= damageAmount;
        hp = Mathf.Max(0, hp);

        Debug.Log($"받은 데미지: {damageAmount}, 남은 HP: {hp}");

        UpdateHealthUI();

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(HitEffect(hitColor));
        }
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

        if (sprite1 != null) sprite1.color = Color.white;
        if (sprite2 != null) sprite2.color = Color.white;
    }
}
