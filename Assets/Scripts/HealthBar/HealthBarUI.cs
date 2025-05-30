using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text nameText;

    public PlayerTest hpSource;

    /// <summary>
    /// 체력 UI 초기화
    /// </summary>
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
        }

        UpdateUI();
    }

    void Update()
    {
        if (hpSource != null)
            UpdateUI();
    }

    private void UpdateUI()
    {
        int cur = hpSource.hp;
        healthSlider.value = cur;
        healthText.text = $"{cur} / {hpSource.MaxHp}";
    }

    /// <summary>
    /// 다른 HealthBarUI에서 모든 정보를 복사
    /// </summary>
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
        }

        UpdateUI();
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
}
