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
        hpBarImage.color = GetColorByHPRatio(hpRatio);
    }

    private Color GetColorByHPRatio(float ratio)
    {
        Color cyan = new Color(0f, 1f, 1f);        // 100%
        Color lime = new Color(0.5f, 1f, 0.5f);    // ~75%
        Color yellow = new Color(1f, 1f, 0f);      // ~50%
        Color orange = new Color(1f, 0.5f, 0f);    // ~25%
        Color red = new Color(1f, 0f, 0f);         // 0%

        if (ratio > 0.75f)
        {
            // 75~100%: lime ¡æ cyan
            float t = (ratio - 0.75f) / 0.25f;
            return Color.Lerp(lime, cyan, t);
        }
        else if (ratio > 0.5f)
        {
            // 50~75%: yellow ¡æ lime
            float t = (ratio - 0.5f) / 0.25f;
            return Color.Lerp(yellow, lime, t);
        }
        else if (ratio > 0.25f)
        {
            // 25~50%: orange ¡æ yellow
            float t = (ratio - 0.25f) / 0.25f;
            return Color.Lerp(orange, yellow, t);
        }
        else
        {
            // 0~25%: red ¡æ orange
            float t = ratio / 0.25f;
            return Color.Lerp(red, orange, t);
        }
    }
}
