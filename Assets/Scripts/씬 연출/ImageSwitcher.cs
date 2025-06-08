using UnityEngine;
using UnityEngine.UI;

public class ImageSwitcher : MonoBehaviour
{
    [Header("�̹��� ���")]
    [SerializeField] private Image targetImage;

    [Header("��������Ʈ ���")]
    [SerializeField] private Sprite[] sprites = new Sprite[4];

    [Header("����� ��ư")]
    [SerializeField] private Button resetButton;

    private int currentIndex = 0;
    private bool isFinished = false;

    void Start()
    {
        if (targetImage != null && sprites.Length > 0)
        {
            targetImage.sprite = sprites[0];
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetImageSwitcher);
        }
    }

    void Update()
    {
        if (isFinished) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (sprites.Length == 0 || targetImage == null) return;

            currentIndex++;

            if (currentIndex >= sprites.Length)
            {
                targetImage.gameObject.SetActive(false);
                Time.timeScale = 1f;
                isFinished = true;
                return;
            }

            targetImage.sprite = sprites[currentIndex];
        }
    }

    public void ResetImageSwitcher()
    {
        if (sprites.Length == 0 || targetImage == null) return;

        currentIndex = 0;
        isFinished = false;

        targetImage.sprite = sprites[0];
        targetImage.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}
