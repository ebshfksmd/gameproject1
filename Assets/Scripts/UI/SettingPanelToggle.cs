using UnityEngine;

public class SettingPanelToggle : MonoBehaviour
{
    [Header("토글할 설정 패널")]
    [SerializeField] private GameObject settingPanel;

    void Update()
    {
        // ESC 키로 설정 패널 끄기
        if (settingPanel != null && settingPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingPanel();
        }
    }

    /// <summary>
    /// 버튼 클릭 시 호출: 설정 패널을 토글함
    /// </summary>
    public void ToggleSettingPanel()
    {
        if (settingPanel != null)
        {
            bool currentState = settingPanel.activeSelf;
            settingPanel.SetActive(!currentState);

            if (!currentState)
            {
                // 설정 패널이 켜질 때 게임 정지
                Time.timeScale = 0f;
            }
            else
            {
                // 설정 패널이 꺼질 때 게임 재개
                Time.timeScale = 1f;
            }
        }
        else
        {
            Debug.LogWarning("SettingPanel이 연결되지 않았습니다.");
        }
    }

    /// <summary>
    /// ESC 키로 설정 패널을 끌 때 호출됨
    /// </summary>
    private void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
