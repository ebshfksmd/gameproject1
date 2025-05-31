using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WalkAndScaleController : MonoBehaviour
{
    public Animator animator;
    public float standSpeed = 0.5f;
    public float walkAnimSpeed = 1.2f;
    public Vector3 standScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 walkScale = Vector3.one;
    public float walkSpeed = 2f;
    public DialogueManager dialogueManager;
    public SpriteRenderer spriteRenderer;

    [Header("Camera")]
    public Camera mainCamera;
    public float cameraMaxX = 8f;

    [Header("Movement Limit")]
    public float characterMaxX = 6f;

    [Header("Sound")]
    public AudioClip walkSFX;

    private AudioSource walkAudioSource; // 걷기 전용 AudioSource

    [Header("Custom Position for 'LH'")]
    public Vector3 customFinalPosition;

    [Header("Next Dialogue Manager")]
    public GameObject nextDialogueManagerObject;

    private bool isWalkingStarted = false;
    private bool isWalkingEnded = false;
    private bool wasOriginallyFlipped = false;
    private float originalY;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (mainCamera == null)
            mainCamera = Camera.main;

        // 걷기용 전용 AudioSource 생성
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;
        walkAudioSource.volume = 0.3f;

        wasOriginallyFlipped = spriteRenderer.flipX;
        originalY = transform.position.y;
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!dialogueManager.IsDialogueFinished)
        {
            if (stateInfo.IsName("stand"))
            {
                animator.speed = standSpeed;
                transform.localScale = standScale;

                Vector3 pos = transform.position;
                pos.y = originalY;
                transform.position = pos;
            }
            else
            {
                animator.speed = 1f;
                transform.localScale = Vector3.one;
            }
        }
        else if (!isWalkingEnded)
        {
            if (!isWalkingStarted)
            {
                animator.SetBool("isWalking", true);
                if (dialogueManager != null)
                    dialogueManager.gameObject.SetActive(false);

                spriteRenderer.flipX = false;
                animator.speed = walkAnimSpeed;
                transform.localScale = walkScale;
                isWalkingStarted = true;

                if (walkSFX != null && walkAudioSource != null)
                {
                    walkAudioSource.clip = walkSFX;
                    walkAudioSource.Play();
                }
            }

            animator.speed = walkAnimSpeed;

            Vector3 pos = transform.position;
            if (pos.x < characterMaxX)
            {
                pos.x += walkSpeed * Time.deltaTime / 1.5f;
                if (pos.x >= characterMaxX)
                {
                    pos.x = characterMaxX;
                    transform.position = pos;
                    EndWalking();
                }
                else
                {
                    pos.y = -17.73f;
                    transform.position = pos;
                }
            }
            else
            {
                EndWalking();
            }

            Vector3 camPos = mainCamera.transform.position;
            if (camPos.x < cameraMaxX)
            {
                camPos.x += walkSpeed * Time.deltaTime / 6f;
                camPos.x = Mathf.Min(camPos.x, cameraMaxX);
                mainCamera.transform.position = camPos;
            }
        }
    }

    private void EndWalking()
    {
        isWalkingEnded = true;
        animator.SetBool("isWalking", false);
        animator.speed = standSpeed;
        transform.localScale = standScale;

        if (gameObject.name == "LH")
            transform.position = customFinalPosition;
        else
        {
            Vector3 pos = transform.position;
            pos.y = originalY;
            transform.position = pos;
        }

        if (wasOriginallyFlipped)
            spriteRenderer.flipX = true;

        if (walkAudioSource != null && walkAudioSource.isPlaying)
            walkAudioSource.Stop();

        if (nextDialogueManagerObject != null)
            StartCoroutine(StartNextDialogueDelayed());
    }

    private IEnumerator StartNextDialogueDelayed()
    {
        yield return null;
        DialogueManager nextManager = nextDialogueManagerObject.GetComponent<DialogueManager>();
        if (nextManager != null)
            nextManager.StartDialogue();
        nextDialogueManagerObject.SetActive(true);
    }
}
