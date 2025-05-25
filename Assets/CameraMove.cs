using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraMove : MonoBehaviour
{
    public float speed = 5f; // 1초에 이동할 거리 (n값)

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}