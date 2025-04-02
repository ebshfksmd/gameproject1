using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    private float moveSpeed = 5f; // 좌우 이동 속도
    private Rigidbody2D rb;
    private Animator animator;
    private float jumpPower = 300;

    private bool isJumping = false;

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
        //카메라
        playerCamera.transform.position = transform.position + new Vector3(2, 2, -10);

        // 좌우 이동 입력 받기 (A, D 또는 좌우 화살표)
        float moveInput = Input.GetAxis("Horizontal");

        //방향키를 입력하고 있을때만 속도계산
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // 캐릭터 방향 전환: 오른쪽 이동이면 스케일 x를 1, 왼쪽이면 -1로 설정
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // 이동 애니메이션 제어: "Speed" 파라미터에 절대값 입력
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        //달리기
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 8;
        }
        else
        {
            moveSpeed = 5;
        }
        //방향키에서 손을 뗏을때 속도 0
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(0,0);
        }
        //점프
        if (Input.GetKeyDown(KeyCode.Space)&& isJumping==false)
        {
            rb.AddForce((transform.up) * jumpPower);
            isJumping = true;
        }
    }

    // 충돌시 isJumping 변수 false
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;
    }
}