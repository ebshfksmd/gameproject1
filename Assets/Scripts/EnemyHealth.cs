using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 50;
    private int currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"{gameObject.name} took {dmg} damage, HP left: {currentHP}");
        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        // »ç¸Á Ã³¸®(ÀÌÆåÆ®, ÆÄ±« µî)
        Destroy(gameObject);
    }
}