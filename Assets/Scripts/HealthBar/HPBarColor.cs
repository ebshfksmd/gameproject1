using UnityEngine;
using UnityEngine.UI;

public class HPBarColor : MonoBehaviour
{
    public Image hpBarImage;
    public float maxHP = 100f;
    public float currentHP = 100f;

    void Update()
    {
        float hpRatio = Mathf.Clamp01(currentHP / maxHP);

        Color color;
        if (hpRatio > 0.5f)
        {
            // 청록색 → 노란색
            float t = (hpRatio - 0.5f) * 2f; // 1~0
            color = Color.Lerp(Color.yellow, Color.cyan, t);
        }
        else
        {
            // 노란색 → 빨간색
            float t = hpRatio * 2f; // 1~0
            color = Color.Lerp(Color.red, Color.yellow, t);
        }

        hpBarImage.color = color;
    }
}