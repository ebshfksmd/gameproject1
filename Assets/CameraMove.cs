using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraMove : MonoBehaviour
{
    public float speed = 5f; // 1�ʿ� �̵��� �Ÿ� (n��)

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}