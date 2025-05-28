using UnityEngine;

public class HPBarFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 몬스터 본체
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0); // HP바 위치 오프셋

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (target == null)
        {
            Debug.LogError("HPBarFollow: Target is not assigned.");
        }
    }

    void Update()
    {
        if (target == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);
        rectTransform.position = screenPos;
    }
}
