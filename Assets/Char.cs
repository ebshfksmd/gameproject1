using UnityEngine;

public class Char : MonoBehaviour
{
    public float speed = 5f; // 1초에 이동할 거리

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                transform.position += Vector3.right * speed * Time.deltaTime * 2;
            else
                transform.position += Vector3.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.LeftShift))
                transform.position += Vector3.left * speed * Time.deltaTime * 2;
            else
                transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }
}
