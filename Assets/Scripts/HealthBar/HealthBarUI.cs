using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text nameText;

    private Image fillImage;

    public PlayerTest hpSource;

    public void Init(PlayerTest source, Sprite profileSprite, string playerName = "", bool flipX = false)
    {
        hpSource = source;

        if (profileImage != null)
        {
            profileImage.sprite = profileSprite;
            profileImage.rectTransform.localEulerAngles = flipX ? new Vector3(0, 180, 0) : Vector3.zero;
        }

        if (nameText != null && !string.IsNullOrEmpty(playerName))
        {
            nameText.text = playerName;
        }

        if (healthSlider != null && hpSource != null)
        {
            healthSlider.maxValue = hpSource.MaxHp;
            fillImage = healthSlider.fillRect?.GetComponent<Image>();
        }

        UpdateUI();
    }

    void Update()
    {
        if (hpSource != null)
            UpdateUI();
        if (fillImage != null)
        {
            float t = Mathf.PingPong(Time.time, 1f);
            fillImage.color = Color.Lerp(Color.red, Color.green, t);
        }
    }

    private void UpdateUI()
    {
        int cur = hpSource.hp;
        int max = hpSource.MaxHp;

        if (healthSlider != null)
        {
            healthSlider.value = cur;
        }

        if (healthText != null)
        {
            healthText.text = $"{cur} / {max}";
        }

        UpdateHPBarColor(cur, max);

        // 죽었을 경우 회색 처리
        SetGrayscale(cur <= 0);
    }

    private void UpdateHPBarColor(float currentHP, float maxHP)
    {
        if (fillImage == null) return;

        float hpRatio = Mathf.Clamp01(currentHP / maxHP);
        Color color;

        // 정교한 4단계 색상 전환
        Color cyan = new Color(0f, 1f, 1f);
        Color lime = new Color(0.5f, 1f, 0.5f);
        Color yellow = new Color(1f, 1f, 0f);
        Color orange = new Color(1f, 0.5f, 0f);
        Color red = new Color(1f, 0f, 0f);

        if (hpRatio > 0.75f)
            color = Color.Lerp(lime, cyan, (hpRatio - 0.75f) / 0.25f);
        else if (hpRatio > 0.5f)
            color = Color.Lerp(yellow, lime, (hpRatio - 0.5f) / 0.25f);
        else if (hpRatio > 0.25f)
            color = Color.Lerp(orange, yellow, (hpRatio - 0.25f) / 0.25f);
        else
            color = Color.Lerp(red, orange, hpRatio / 0.25f);

        fillImage.color = color;
    }

    public void SetGrayscale(bool isGray)
    {
        if (profileImage != null)
        {
            profileImage.color = isGray ? new Color(0.2f, 0.2f, 0.2f, 1f) : Color.white;
        }
    }

    public void SetProfileImage(Sprite newSprite, bool flipX = false, string newName = "")
    {
        if (profileImage != null)
        {
            profileImage.sprite = newSprite;
            profileImage.rectTransform.localEulerAngles = flipX ? new Vector3(0, 180, 0) : Vector3.zero;
        }

        if (nameText != null && !string.IsNullOrEmpty(newName))
        {
            nameText.text = newName;
        }
    }

    public void CopyFrom(HealthBarUI other)
    {
        this.hpSource = other.hpSource;

        if (profileImage != null && other.profileImage != null)
        {
            profileImage.sprite = other.profileImage.sprite;
            profileImage.rectTransform.localEulerAngles = other.profileImage.rectTransform.localEulerAngles;
        }

        if (nameText != null && other.nameText != null)
        {
            nameText.text = other.nameText.text;
        }

        if (healthSlider != null && other.healthSlider != null)
        {
            healthSlider.maxValue = other.healthSlider.maxValue;
            fillImage = other.healthSlider.fillRect?.GetComponent<Image>();
        }

        UpdateUI();
    }
}
