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
    [SerializeField] private List<GameObject> fadeObjects; // SpriteRenderer, MeshRenderer ��
    [SerializeField] private List<Graphic> fadeTexts; // UI �۾� (Text, Image, TMP ��)
    [SerializeField] private float fadeAlpha = 0.3f;
    [SerializeField] private float normalAlpha = 1.0f;

    private bool isFollowing = true;
    private Transform target;
    private bool isFaded = false;

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
            }
            else if (!isAtLimit && isFaded)
            {
                SetObjectsAlpha(normalAlpha);
                SetTextAlpha(normalAlpha);
                isFaded = false;
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

            // UI Image (��: ProfileImage)
            var uiImage = obj.GetComponent<UnityEngine.UI.Image>();
            if (uiImage != null)
            {
                Color c = uiImage.color;
                c.a = alpha;
                uiImage.color = c;
                continue;
            }

            // �Ϲ� Renderer (SpriteRenderer, MeshRenderer ��)
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
            FindActivePlayer();
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
