using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Player Follow Settings")]
    [SerializeField] private string playerTag = "Player"; // ��� �÷��̾�� �� �±׸� ���
    [SerializeField] private float smoothSpeed = 200f;
    [SerializeField] private Vector3 offset = new Vector3(5f, 0f, -10f); // Y���� ������ ��ġ

    private Transform target;

    void Start()
    {
        Camera.main.orthographicSize = 5f; // or any size you want
    }

    void LateUpdate()
    {
        UpdateCamera(); // �Ʒ� �Լ��� �и��ϸ� �� �� �׽�Ʈ�ϱ� ����
    }

    private void UpdateCamera()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindActivePlayer();
        }

        if (target != null)
        {
            Vector3 currentPosition = transform.position;

            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                offset.y,
                offset.z
            );

            Vector3 smoothed = Vector3.Lerp(currentPosition, desiredPosition, smoothSpeed * Time.fixedDeltaTime); // fixedDeltaTime ���
            transform.position = smoothed;
        }
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
