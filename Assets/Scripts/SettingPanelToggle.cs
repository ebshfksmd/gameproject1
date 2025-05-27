using UnityEngine;

public class SettingPanelToggle : MonoBehaviour
{
    [Header("토글할 설정 패널")]
    [SerializeField] private GameObject settingPanel;

    /// <summary>
    /// 버튼 클릭 시 호출: 설정 패널을 토글함
    /// </summary>
    public void ToggleSettingPanel()
    {
        if (settingPanel != null)
        {
            bool currentState = settingPanel.activeSelf;
            settingPanel.SetActive(!currentState);
        }
        else
        {
            Debug.LogWarning("SettingPanel이 연결되지 않았습니다.");
        }
    }
}
