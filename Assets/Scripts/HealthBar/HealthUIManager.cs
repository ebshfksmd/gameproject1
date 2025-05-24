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
    [SerializeField] private Sprite[] profileSprites; // �ε��� ��Ī

    private int currentIndex = 0;

    void Start()
    {
        UpdateAllBars();
    }

    /// <summary>PlayerSwitcher���� ȣ���ϼ���.</summary>
    public void OnCharacterSwitched(int newIndex)
    {
        currentIndex = newIndex;
        UpdateAllBars();
    }

    private void UpdateAllBars()
    {
        
        // Ŭ����
        foreach (Transform t in leftPanel) Destroy(t.gameObject);
        foreach (Transform t in rightPanel) Destroy(t.gameObject);

        // 1) ����: ���� ĳ����
        var mainChar = characters[currentIndex];
        var mainBar = Instantiate(healthBarPrefab, leftPanel);
        mainBar.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 80);
        mainBar.GetComponent<HealthBarUI>()
            .Init(mainChar, profileSprites[currentIndex]);

        // 2) ������: ������ �������
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