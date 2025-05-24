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
    /// �ʱ�ȭ: �ҽ�, ������ ��������Ʈ, �����̴� �ִ�ġ ����
    /// </summary>
    public void Init(Player_health source, Sprite profileSprite)
    {
        hpSource = source;
        profileImage.sprite = profileSprite;
        healthSlider.maxValue = source.MaxHealth; // MaxHealth ������Ƽ �߰� �ʿ�
        UpdateUI();
    }

    void Update()
    {
        if (hpSource != null)
            UpdateUI();
    }

    private void UpdateUI()
    {
        int cur = hpSource.CurrentHealth; // CurrentHealth ������Ƽ �ʿ�
        healthSlider.value = cur;
        healthText.text = $"{cur} / {hpSource.MaxHealth}";
    }
}