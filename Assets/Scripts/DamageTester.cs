using UnityEngine;

public class DamageTester : MonoBehaviour
{
    [SerializeField] private Player_health health;
    [SerializeField] private int testDamage = 100;

    void Awake()
    {
        if (health == null)
            health = GetComponent<Player_health>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            health.TakeDamage(testDamage);
        }
    }
}
