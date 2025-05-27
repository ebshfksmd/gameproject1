using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoardZoomController : MonoBehaviour
{
    public enum ZoomState { Original, BoardZoom, DeepZoom }

    [Header("References")]
    public Camera targetCamera;
    public Transform zoomTarget;
    public CanvasGroup scrollbarGroup;

    [Header("Zoom Settings")]
    public Vector3 targetOffset = new Vector3(0, 0, -10f);
    public float zoomInSize = 5f;
    private float deepZoomSize = 1f;
    public float duration = 1f;

    [Header("Zoom Objects")]
    [SerializeField] private GameObject boardZoomObject1;
    [SerializeField] private GameObject boardZoomObject2;
    [SerializeField] private GameObject boardZoomObject3;

    [SerializeField] private SpriteRenderer title;
    [SerializeField] private GameObject objectToHide;
    [SerializeField] private GameObject objectToShow;

    [Header("Audio")]
    [SerializeField] private AudioClip playButtonClip;
    [SerializeField] private AudioClip boardZoom1Clip;
    [SerializeField] private AudioClip boardZoom2Clip;
    [SerializeField] private AudioClip boardZoom3Clip;

    [Header("UI for Object3")]
    [SerializeField] private GameObject object3UI1;
    [SerializeField] private GameObject object3UI2;

    private AudioSource audioSource;

    private Vector3 originalPosition;
    private float originalSize;
    private ZoomState zoomState = ZoomState.Original;
    private Coroutine animRoutine;
    private Coroutine scrollbarRoutine;
    private bool isClickedPlayButton = false;

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        originalPosition = targetCamera.transform.position;
        originalSize = targetCamera.orthographicSize;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (scrollbarGroup != null)
        {
            scrollbarGroup.alpha = 0f;
            scrollbarGroup.interactable = false;
            scrollbarGroup.blocksRaycasts = false;
        }

        if (title != null)
        {
            Color c = title.color;
            c.a = 1f;
            title.color = c;
        }

        if (boardZoomObject1 != null) boardZoomObject1.SetActive(false);
        if (boardZoomObject2 != null) boardZoomObject2.SetActive(false);
        if (boardZoomObject3 != null) boardZoomObject3.SetActive(false);

        if (object3UI1 != null) object3UI1.SetActive(false);
        if (object3UI2 != null) object3UI2.SetActive(false);
    }

    private void OnMouseDown()
    {
        isClickedPlayButton = true;

        if (playButtonClip != null)
        {
            audioSource.PlayOneShot(playButtonClip);
        }
    }

    void Update()
    {
        if (zoomState == ZoomState.Original && Input.GetMouseButtonDown(0) && isClickedPlayButton)
        {
            AnimateTo(zoomTarget.position + targetOffset, zoomInSize);
            zoomState = ZoomState.BoardZoom;
            FadeScrollbar(true);

            if (boardZoomObject1 != null) boardZoomObject1.SetActive(true);
            if (boardZoomObject2 != null) boardZoomObject2.SetActive(true);
            if (boardZoomObject3 != null) boardZoomObject3.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (zoomState == ZoomState.DeepZoom)
            {
                AnimateTo(zoomTarget.position + targetOffset, zoomInSize);
                zoomState = ZoomState.BoardZoom;
                FadeScrollbar(true);
                deepZoomSize = 1f;

                if (object3UI1 != null) object3UI1.SetActive(false);
                if (object3UI2 != null) object3UI2.SetActive(false);
            }
            else if (zoomState == ZoomState.BoardZoom)
            {
                AnimateTo(originalPosition, originalSize);
                zoomState = ZoomState.Original;
                FadeScrollbar(false);

                if (boardZoomObject1 != null) boardZoomObject1.SetActive(false);
                if (boardZoomObject2 != null) boardZoomObject2.SetActive(false);
                if (boardZoomObject3 != null) boardZoomObject3.SetActive(false);

                if (object3UI1 != null) object3UI1.SetActive(false);
                if (object3UI2 != null) object3UI2.SetActive(false);
            }
        }

        if (zoomState == ZoomState.DeepZoom && Input.GetKeyDown(KeyCode.Space))
        {
            if (objectToHide != null) objectToHide.SetActive(false);
            if (objectToShow != null) objectToShow.SetActive(true);
        }
    }

    public void DeepZoomTo(Transform target, bool showScrollbar)
    {
        if (zoomState != ZoomState.BoardZoom) return;

        if (boardZoomObject1 != null && target == boardZoomObject1.transform)
        {
            deepZoomSize = 0.77f;
            if (boardZoom1Clip != null) audioSource.PlayOneShot(boardZoom1Clip);
        }
        else if (boardZoomObject2 != null && target == boardZoomObject2.transform)
        {
            deepZoomSize = 1.3f;
            if (boardZoom2Clip != null) audioSource.PlayOneShot(boardZoom2Clip);
        }
        else if (boardZoomObject3 != null && target == boardZoomObject3.transform)
        {
            deepZoomSize = 0.66f;
            if (boardZoom3Clip != null) audioSource.PlayOneShot(boardZoom3Clip);
        }
        else
        {
            deepZoomSize = 1f;
        }

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

        if (zoomState == ZoomState.DeepZoom && Mathf.Approximately(toSize, 0.66f))
        {
            if (object3UI1 != null) object3UI1.SetActive(true);
            if (object3UI2 != null) object3UI2.SetActive(true);
        }
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
        float titleEndAlpha = show ? 0f : 1f;

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
