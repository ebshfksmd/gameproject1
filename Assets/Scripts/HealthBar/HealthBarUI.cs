using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text nameText;

    [Header("수동 색상 전환용")]
    [SerializeField] private Image[] hpBarSegments; // 최대 4개

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

        if (!string.IsNullOrEmpty(playerName) && nameText != null)
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
            Debug.Log("Fill color: " + fillImage.color);
        }
    }

    private void UpdateUI()
    {
        int cur = hpSource.hp;
        int max = hpSource.MaxHp;

        if (healthSlider != null)
            healthSlider.value = cur;

        if (healthText != null)
            healthText.text = $"{cur} / {max}";

        UpdateSegmentColors(cur, max);
        SetGrayscale(cur <= 0);
    }

    private void UpdateSegmentColors(int cur, int max)
    {
        float ratio = (float)cur / max;

        // 4단계 색상 기준
        Color cyan = new Color(0f, 1f, 1f);       // 100%
        Color lime = new Color(0.5f, 1f, 0.5f);   // 75~100%
        Color yellow = new Color(1f, 1f, 0f);     // 50~75%
        Color orange = new Color(1f, 0.5f, 0f);   // 25~50%
        Color red = new Color(1f, 0f, 0f);        // 0~25%

        for (int i = 0; i < hpBarSegments.Length; i++)
        {
            if (hpBarSegments[i] == null) continue;

            float threshold = (i + 1) / 4f;  // ex: 0.25, 0.5, 0.75, 1.0
            Color segmentColor;

            if (ratio > 0.75f)
                segmentColor = Color.Lerp(lime, cyan, (ratio - 0.75f) / 0.25f);
            else if (ratio > 0.5f)
                segmentColor = Color.Lerp(yellow, lime, (ratio - 0.5f) / 0.25f);
            else if (ratio > 0.25f)
                segmentColor = Color.Lerp(orange, yellow, (ratio - 0.25f) / 0.25f);
            else
                segmentColor = Color.Lerp(red, orange, ratio / 0.25f);

            hpBarSegments[i].color = ratio >= threshold ? segmentColor : Color.gray;
        }
    }


    public void SetGrayscale(bool isGray)
    {
        if (profileImage != null)
            profileImage.color = isGray ? new Color(0.2f, 0.2f, 0.2f, 1f) : Color.white;
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

        if (other.hpBarSegments != null && hpBarSegments != null)
        {
            for (int i = 0; i < hpBarSegments.Length && i < other.hpBarSegments.Length; i++)
            {
                hpBarSegments[i] = other.hpBarSegments[i];
            }
        }

        UpdateUI();
    }
}
