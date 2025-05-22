using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Animator anim;
    private float moveInput = 0;
    private bool isGrounded = true;

    private Rigidbody2D rb;
    [Header("Player Movement")]
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float runSpeed = 1.4f;
    [SerializeField]
    private float Scale = 1f;
    private float speedMultiplier = 1f;
    private Coroutine buffRoutine;


    void Awake()
    {
        transform.localScale = new Vector3(Scale, Scale, Scale);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on player object.");
        }
    }

    void Update()
    {
        PlayerMovement();
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (col.collider.CompareTag("Hitbox"))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hitbox"))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void PlayerMovement()
    {
        moveInput = Input.GetAxis("Horizontal") * speedMultiplier;
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.localScale = new Vector3(Scale, Scale, Scale);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            transform.localScale = new Vector3(-Scale, Scale, Scale);
        }
        // Move the player left or right
        // Check if the player is pressing the left or right arrow keys or A/D keys
        // and set the linear velocity accordingly and stop the player when the keys are released
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isWalking", true);
            //run
            //Check if the player is pressing the shift key and set the linear velocity accordingly
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                anim.SetBool("isRun", true);
                rb.linearVelocity = new Vector2(moveInput * moveSpeed * runSpeed, rb.linearVelocity.y);
            }
            else
            {
                anim.SetBool("isRun", false);
                rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            }
        }
        else anim.SetBool("isWalking", false);

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Jump
        // Check if the player is on the ground before allowing a jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            anim.Play("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        // 이미 버프 중이면 갱신
        if (buffRoutine != null)
            StopCoroutine(buffRoutine);
        buffRoutine = StartCoroutine(SpeedBuffCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBuffCoroutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
        buffRoutine = null;
    }

}
