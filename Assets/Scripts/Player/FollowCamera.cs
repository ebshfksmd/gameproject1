using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Player Follow Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(5f, 0f, -10f);

    [Header("Idle Camera Position")]
    [SerializeField] private Vector3 fixedPosition = new Vector3(-46.93389f, -0.078476f, -9.719418f); // 고정 위치

    private bool isFollowing = true;
    private Transform target;

    void Start()
    {
        Camera.main.orthographicSize = 5f;
    }

    void LateUpdate()
    {
        if (isFollowing)
            UpdateCamera();
        else
            MoveToFixedPosition();
    }

    private void UpdateCamera()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindActivePlayer();
        }

        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                offset.y,
                offset.z
            );

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        }
    }

    private void MoveToFixedPosition()
    {
        transform.position = fixedPosition;
    }

    public void SetFollow(bool follow)
    {
        isFollowing = follow;
        Debug.Log("카메라 모드: " + (follow ? "플레이어 추적" : "고정 위치"));
        if (follow)
            FindActivePlayer();
    }

    void FindActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy)
            {
                target = p.transform;
                break;
            }
        }
    }
}
