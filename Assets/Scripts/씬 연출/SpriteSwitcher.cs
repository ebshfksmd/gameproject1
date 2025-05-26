using UnityEngine;

public class SpriteSwitcher : MonoBehaviour
{

    public Sprite hoverSprite;     // ���콺 ���� �� ������ ��������Ʈ
    private Sprite originalSprite; // ���� ��������Ʈ

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