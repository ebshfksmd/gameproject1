using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 좌우 이동 속도
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 컴포넌트가 없습니다!");
        }
        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트가 없습니다!");
        }
    }

    void Update()
    {
        // 좌우 이동 입력 받기 (A, D 또는 좌우 화살표)
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 이동 애니메이션 제어: "Speed" 파라미터에 절대값 입력
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // 캐릭터 방향 전환: 오른쪽 이동이면 스케일 x를 1, 왼쪽이면 -1로 설정
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}