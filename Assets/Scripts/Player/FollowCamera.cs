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
        // 시작할 때 모두 투명도 1로 설정
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
                Debug.Log("[조건 만족] SetFollow(true) 호출");
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
                Debug.Log("[UpdateCamera] 벽 고정 → 투명도 적용");
            }
            else if (!isAtLimit && isFaded)
            {
                SetObjectsAlpha(normalAlpha);
                SetTextAlpha(normalAlpha);
                isFaded = false;
                Debug.Log("[UpdateCamera] 벽 고정 해제 → 투명도 복원");
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
        Debug.Log("카메라 모드: " + (follow ? "플레이어 추적" : "고정 위치"));

        if (follow)
        {
            FindActivePlayer();
            Debug.Log("[SetFollow] 추적 모드 진입");

            if (isFaded)
            {
                Debug.Log("[SetFollow] 벽 고정 상태 초기화 시작");
                SetObjectsAlpha(normalAlpha);
                SetTextAlpha(normalAlpha);
                isFaded = false;
                Debug.Log("[SetFollow] 투명도 복구 완료");
            }
            else
            {
                Debug.Log("[SetFollow] isFaded == false, 초기화 생략");
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
                Debug.Log($"[플레이어 찾음] {p.name}");
                break;
            }
        }

        if (target == null)
            Debug.LogWarning("[경고] 활성 플레이어를 찾지 못했습니다.");
    }
}
