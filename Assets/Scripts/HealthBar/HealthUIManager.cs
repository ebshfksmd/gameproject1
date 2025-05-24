using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    [Header("UI Containers")]
    [SerializeField] private Transform leftPanel;
    [SerializeField] private Transform rightPanel;

    [Header("HealthBar Prefab")]
    [SerializeField] private GameObject healthBarPrefab;

    [Header("Characters")]
    [SerializeField] private Player_health[] characters;
    [SerializeField] private Sprite[] profileSprites; // 인덱스 매칭

    private int currentIndex = 0;

    void Start()
    {
        UpdateAllBars();
    }

    /// <summary>PlayerSwitcher에서 호출하세요.</summary>
    public void OnCharacterSwitched(int newIndex)
    {
        currentIndex = newIndex;
        UpdateAllBars();
    }

    private void UpdateAllBars()
    {
        
        // 클리어
        foreach (Transform t in leftPanel) Destroy(t.gameObject);
        foreach (Transform t in rightPanel) Destroy(t.gameObject);

        // 1) 왼쪽: 현재 캐릭터
        var mainChar = characters[currentIndex];
        var mainBar = Instantiate(healthBarPrefab, leftPanel);
        mainBar.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 80);
        mainBar.GetComponent<HealthBarUI>()
            .Init(mainChar, profileSprites[currentIndex]);

        // 2) 오른쪽: 나머지 순서대로
        int count = characters.Length;
        for (int i = 1; i < count; i++)
        {
            int idx = (currentIndex + i) % count;
            var bar = Instantiate(healthBarPrefab, rightPanel);
            bar.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 50);
            bar.GetComponent<HealthBarUI>()
                .Init(characters[idx], profileSprites[idx]);
        }
    }
}