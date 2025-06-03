using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FollowCamera : MonoBehaviour
{
    [Header("Player Follow Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(5f, 0f, -10f);

    [Header("Idle Camera Position")]
    [SerializeField] private Vector3 fixedPosition = new Vector3(0, 0, -10f);

    [Header("Wall Limits")]
    [SerializeField] private float leftLimitX = -60f;
    [SerializeField] private float rightLimitX = 60.1f;

    [Header("Fade Targets")]
    [SerializeField] private List<GameObject> fadeObjects;
    [SerializeField] private List<Graphic> fadeTexts;
    [SerializeField] private float fadeAlpha = 0.3f;
    [SerializeField] private float normalAlpha = 1.0f;

    [Header("Auto Unlock")]
    [SerializeField] private bool enableAutoUnlock = true;
    [SerializeField] private GameObject objectA;
    [SerializeField] private GameObject objectB;

    private bool isFollowing = true;
    private Transform target;
    private bool isFaded = false;

    void Start()
    {
        // ������ �� ��� ���� 1�� ����
        SetObjectsAlpha(normalAlpha);
        SetTextAlpha(normalAlpha);
    }

    void LateUpdate()
    {
        if (enableAutoUnlock && !isFollowing)
        {
            bool aInactive = objectA == null || !objectA.activeInHierarchy;
            bool bActive = objectB != null && objectB.activeInHierarchy;

            if (aInactive && bActive)
            {
                Debug.Log("[���� ����] SetFollow(true) ȣ��");
                SetFollow(true);
            }
        }

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

            float halfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float minX = leftLimitX + halfWidth;
            float maxX = rightLimitX - halfWidth;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

            bool isAtLimit = desiredPosition.x <= minX || desiredPosition.x >= maxX;

            if (isAtLimit && !isFaded)
            {
                SetObjectsAlpha(fadeAlpha);
                SetTextAlpha(fadeAlpha);
                isFaded = true;
                Debug.Log("[UpdateCamera] �� ���� �� ���� ����");
            }
            else if (!isAtLimit && isFaded)
            {
                SetObjectsAlpha(normalAlpha);
                SetTextAlpha(normalAlpha);
                isFaded = false;
                Debug.Log("[UpdateCamera] �� ���� ���� �� ���� ����");
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }

    private void SetObjectsAlpha(float alpha)
    {
        foreach (GameObject obj in fadeObjects)
        {
            if (obj == null) continue;

            var uiImage = obj.GetComponent<UnityEngine.UI.Image>();
            if (uiImage != null)
            {
                Color c = uiImage.color;
                c.a = alpha;
                uiImage.color = c;
                continue;
            }

            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.material.HasProperty("_Color"))
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }
        }
    }

    private void SetTextAlpha(float alpha)
    {
        foreach (Graphic graphic in fadeTexts)
        {
            if (graphic == null) continue;

            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }
    }

    private void MoveToFixedPosition()
    {
        transform.position = fixedPosition;
    }

    public void SetFollow(bool follow)
    {
        isFollowing = follow;
        Debug.Log("ī�޶� ���: " + (follow ? "�÷��̾� ����" : "���� ��ġ"));

        if (follow)
        {
            FindActivePlayer();
            Debug.Log("[SetFollow] ���� ��� ����");

            if (isFaded)
            {
                Debug.Log("[SetFollow] �� ���� ���� �ʱ�ȭ ����");
                SetObjectsAlpha(normalAlpha);
                SetTextAlpha(normalAlpha);
                isFaded = false;
                Debug.Log("[SetFollow] ���� ���� �Ϸ�");
            }
            else
            {
                Debug.Log("[SetFollow] isFaded == false, �ʱ�ȭ ����");
            }
        }
    }

    private void FindActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy)
            {
                target = p.transform;
                Debug.Log($"[�÷��̾� ã��] {p.name}");
                break;
            }
        }

        if (target == null)
            Debug.LogWarning("[���] Ȱ�� �÷��̾ ã�� ���߽��ϴ�.");
    }
}
