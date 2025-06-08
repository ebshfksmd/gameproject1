using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverAndHideImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("대상 버튼 이미지")]
    [SerializeField] private Image buttonImage;

    [Header("기본/호버 스프라이트")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;

    [Header("비활성화할 오브젝트")]
    [SerializeField] private GameObject targetToHide;

    void Start()
    {
        if (buttonImage != null && defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null && hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null && defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetToHide != null)
        {
            buttonImage.sprite = defaultSprite;
            targetToHide.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
