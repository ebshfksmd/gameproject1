using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Animator anim;
    private bool isGrounded = true;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private AudioSource sfxAudioSource;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeed = 1.4f;
    [SerializeField] private float Scale = 1f;
    private float speedMultiplier = 1f;
    private Coroutine buffRoutine;

    [Header("Audio")]
    public AudioClip isRun;
    public AudioClip isWalking;
    public AudioClip jump;

    public bool canControl = true;

    private float fixedX;

    public bool IsGrounded => isGrounded;

    void Awake()
    {
        transform.localScale = new Vector3(Scale, Scale, Scale);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.volume = 1f;

        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.loop = false;
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.volume = 1f;

        if (rb == null)
            Debug.LogError("Rigidbody2D component not found on player object.");
    }

    void Start()
    {
        fixedX = transform.position.x;
    }

    void Update()
    {
        if (!canControl || PlayerSkillController.canAttack == false) return;

        float input = Input.GetAxis("Horizontal");
        float moveAmount = input * moveSpeed * speedMultiplier * Time.deltaTime;

        if (moveAmount != 0f)
        {
            transform.Translate(new Vector3(moveAmount, 0f, 0f));
            fixedX = transform.position.x;

            if (input > 0)
                transform.localScale = new Vector3(Scale, Scale, Scale);
            else if (input < 0)
                transform.localScale = new Vector3(-Scale, Scale, Scale);
        }

        bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        anim.SetBool("isWalking", input != 0);
        anim.SetBool("isRun", input != 0 && isShift);

        if (input != 0)
        {
            PlaySoundIfNotPlaying(isShift ? isRun : isWalking);
        }
        else
        {
            StopSoundIfPlaying(isWalking);
            StopSoundIfPlaying(isRun);
        }

        if (canControl && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            anim.Play("jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;

            if (jump != null)
                sfxAudioSource.PlayOneShot(jump);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hitbox"))
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else
        {
            anim.SetBool("isRun", false);
            anim.SetBool("isWalking", false);
        }
    }

    private void PlaySoundIfNotPlaying(AudioClip clip)
    {
        if (clip == null) return;

        if (!audioSource.isPlaying || audioSource.clip != clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StopSoundIfPlaying(AudioClip clip)
    {
        if (audioSource.isPlaying && audioSource.clip == clip)
        {
            audioSource.Stop();
        }
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
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
