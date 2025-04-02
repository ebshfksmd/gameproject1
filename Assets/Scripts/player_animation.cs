using UnityEngine;

public class player_animation : MonoBehaviour
{
    [SerializeField]
    private Animator[] childAnimator;


    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        for (int i = childAnimator.Length - 1; i >= 0; i--)
        {
            if (childAnimator[i].gameObject.activeInHierarchy)
            {
                childAnimator[i].SetBool("isRunning", moveInput != 0);
            }
        }
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
