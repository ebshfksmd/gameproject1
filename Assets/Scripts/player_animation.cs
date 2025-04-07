using UnityEngine;

public class player_animation : MonoBehaviour
{
    [SerializeField]
    private Animator[] childAnimator;
    private PlayerMovement pmove;
    [SerializeField]
    private Transform[] childSpriteTransform;
    void Start()
    {
        pmove = GetComponentInParent<PlayerMovement>();
        if (pmove == null)
        {
            Debug.LogError("PlayerMovement ������Ʈ�� �����ϴ�!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (pmove.canmove)
        {
            float moveInput = Input.GetAxis("Horizontal");

            for (int i = childAnimator.Length - 1; i >= 0; i--)
            {
                if (childAnimator[i].gameObject.activeInHierarchy)
                {
                    childAnimator[i].SetBool("isMove", moveInput != 0);
                }
            }
            for(int i = childSpriteTransform.Length - 1; i >= 0; i--)
            {
                if (moveInput < 0)
                {
                    childSpriteTransform[i].localScale = new Vector3(1, 1, 1);
                }
                else if (moveInput > 0)
                {
                    childSpriteTransform[i].localScale = new Vector3(-1, 1, 1);
                }
            }
        }

    }

    public void setRunning(bool isRunning)
    {
        for (int i = childAnimator.Length - 1; i >= 0; i--)
        {
            if (childAnimator[i].gameObject.activeInHierarchy)
            {
                childAnimator[i].SetBool("isRunning", isRunning);
            }
        }
    }
}
