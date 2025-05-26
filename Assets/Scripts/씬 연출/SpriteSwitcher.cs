using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{

    public Sprite hoverSprite;     // 마우스 오버 시 보여줄 스프라이트
    private Sprite originalSprite; // 원래 스프라이트

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
    }

    void OnMouseEnter()
    {
        spriteRenderer.sprite = hoverSprite;
    }

    void OnMouseExit()
    {
        spriteRenderer.sprite = originalSprite;
    }
}