using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Handles multi-step camera zoom and move in Unity 2D with ESC rollback and conditional scrollbar fade-in/out on deep zoom.
/// </summary>
public class BoardZoomController : MonoBehaviour
{
    public enum ZoomState { Original, BoardZoom, DeepZoom }

    [Header("References")]
    public Camera targetCamera;
    public Transform zoomTarget;
    public CanvasGroup scrollbarGroup;

    [Header("Settings")]
    public Vector3 targetOffset = new Vector3(0, 0, -10f);
    public float zoomInSize = 5f;
    public float deepZoomSize = 2f;
    public float duration = 1f;

    private Vector3 originalPosition;
    private float originalSize;
    private ZoomState zoomState = ZoomState.Original;
    private Coroutine animRoutine;
    private Coroutine scrollbarRoutine;

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
        originalPosition = targetCamera.transform.position;
        originalSize = targetCamera.orthographicSize;

        if (scrollbarGroup != null)
        {
            scrollbarGroup.alpha = 0f;
            scrollbarGroup.interactable = false;
            scrollbarGroup.blocksRaycasts = false;
        }

        Color c = title.color;
        c.a = 1f;
        title.color = c;
    }



    [SerializeField] SpriteRenderer title;
    [Header("게임 입장 버튼")]
    [SerializeField] GameObject gameEnterButton;
    bool isClickedPlayButton = false;
    private void OnMouseDown()
    {
        isClickedPlayButton = true;
    }
    void Update()
    {
        if (zoomState == ZoomState.Original && Input.GetMouseButtonDown(0) && isClickedPlayButton)
        {
            AnimateTo(zoomTarget.position + targetOffset, zoomInSize);
            zoomState = ZoomState.BoardZoom;

            FadeScrollbar(true);  // 타이틀 사라짐, 스크롤바 나타남
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (zoomState == ZoomState.DeepZoom)
            {
                AnimateTo(zoomTarget.position + targetOffset, zoomInSize);
                zoomState = ZoomState.BoardZoom;

                FadeScrollbar(true);  // 타이틀 사라짐, 스크롤바 나타남
            }
            else if (zoomState == ZoomState.BoardZoom)
            {
                AnimateTo(originalPosition, originalSize);
                zoomState = ZoomState.Original;

                FadeScrollbar(false);  // 타이틀 나타남, 스크롤바 사라짐
                gameEnterButton.SetActive(false);
            }
        }
    }



    /// <summary>
    /// Deep zoom to target. showScrollbar flag controls scrollbar appearance.
    /// </summary>
    public void DeepZoomTo(Transform target, bool showScrollbar)
    {
        if (zoomState != ZoomState.BoardZoom) return;
        AnimateTo(target.position + targetOffset, deepZoomSize);
        zoomState = ZoomState.DeepZoom;
        if (showScrollbar)
            FadeScrollbar(true);
    }

    private void AnimateTo(Vector3 pos, float size)
    {
        if (animRoutine != null)
            StopCoroutine(animRoutine);
        Vector3 startPos = targetCamera.transform.position;
        float startSize = targetCamera.orthographicSize;
        animRoutine = StartCoroutine(AnimateCoroutine(startPos, pos, startSize, size));
    }


    private IEnumerator AnimateCoroutine(Vector3 fromPos, Vector3 toPos, float fromSize, float toSize)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetCamera.transform.position = Vector3.Lerp(fromPos, toPos, t);
            targetCamera.orthographicSize = Mathf.Lerp(fromSize, toSize, t);

            elapsed += Time.deltaTime;
            yield return null;
        }
        targetCamera.transform.position = toPos;
        targetCamera.orthographicSize = toSize;

        isClickedPlayButton = false;
    }


    private void FadeScrollbar(bool show)
    {
        if (scrollbarGroup == null) return;
        if (scrollbarRoutine != null)
            StopCoroutine(scrollbarRoutine);
        scrollbarRoutine = StartCoroutine(FadeCoroutine(show));
    }

    private IEnumerator FadeCoroutine(bool show)
    {
        float startAlpha = scrollbarGroup.alpha;
        float endAlpha = show ? 1f : 0f;

        Color titleColor = title.color;
        float titleStartAlpha = titleColor.a;
        float titleEndAlpha = show ? 0f : 1f;  // 스크롤바 보이면 타이틀 사라지고, 아니면 보임

        float elapsed = 0f;

        if (show)
        {
            scrollbarGroup.interactable = true;
            scrollbarGroup.blocksRaycasts = true;
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            scrollbarGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            titleColor.a = Mathf.Lerp(titleStartAlpha, titleEndAlpha, t);
            title.color = titleColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        scrollbarGroup.alpha = endAlpha;
        titleColor.a = titleEndAlpha;
        title.color = titleColor;

        if (!show)
        {
            scrollbarGroup.interactable = false;
            scrollbarGroup.blocksRaycasts = false;
        }
    }



}
