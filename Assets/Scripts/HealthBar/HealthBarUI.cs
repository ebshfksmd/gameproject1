using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private Player_health hpSource;

    /// <summary>
    /// 초기화: 소스, 프로필 스프라이트, 슬라이더 최대치 세팅
    /// </summary>
    public void Init(Player_health source, Sprite profileSprite)
    {
        hpSource = source;
        profileImage.sprite = profileSprite;
        healthSlider.maxValue = source.MaxHealth; // MaxHealth 프로퍼티 추가 필요
        UpdateUI();
    }

    void Update()
    {
        if (hpSource != null)
            UpdateUI();
    }

    private void UpdateUI()
    {
        int cur = hpSource.CurrentHealth; // CurrentHealth 프로퍼티 필요
        healthSlider.value = cur;
        healthText.text = $"{cur} / {hpSource.MaxHealth}";
    }
}