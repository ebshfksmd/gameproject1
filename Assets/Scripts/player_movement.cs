using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    GameObject playercamera;
    private Rigidbody2D rb;
    private Animator animator;
    private player_animation playerAnimation;

    private float jumpPower = 300;
    private float moveSpeed = 5f;

    public bool canmove = true;
    private bool isJumping = false;

    void Start()
    {
        playerAnimation = GetComponentInChildren<player_animation>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D 컴포넌트가 없습니다!");
        }
        if(playerAnimation == null)
                   {
            Debug.LogError("PlayerAnimation 컴포넌트가 없습니다!");
        }
    }

    public void setCanMove(bool canMove)
    {
        canmove = canMove;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        if (canmove) move(moveInput);
        else stopmove();
    }

    // 충돌시 isJumping 변수 false
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;
    }

    public void move(float moveInput)
    {
        // 좌우 이동 입력 받기 (A, D 또는 좌우 화살표)

        //방향키를 입력하고 있을때만 속도계산
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        //달리기
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 8;
            playerAnimation.setRunning(true);
        }
        else
        {
            moveSpeed = 5;
            playerAnimation.setRunning(false);
        }

        //방향키에서 손을 뗏을때 속도 0
        if (Mathf.Abs(moveInput) < 0.01f)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        //점프
        if (Input.GetKeyDown(KeyCode.Space) && isJumping == false)
        {
            rb.AddForce((transform.up) * jumpPower);
            isJumping = true;
        }
        
    }
    public void stopmove()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}