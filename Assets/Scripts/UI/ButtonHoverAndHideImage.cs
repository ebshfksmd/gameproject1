using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverAndHideImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("��� ��ư �̹���")]
    [SerializeField] private Image buttonImage;

    [Header("�⺻/ȣ�� ��������Ʈ")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;

    [Header("��Ȱ��ȭ�� ������Ʈ")]
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
