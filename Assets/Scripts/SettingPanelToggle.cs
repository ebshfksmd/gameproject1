using UnityEngine;

public class SettingPanelToggle : MonoBehaviour
{
    [Header("����� ���� �г�")]
    [SerializeField] private GameObject settingPanel;

    /// <summary>
    /// ��ư Ŭ�� �� ȣ��: ���� �г��� �����
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
            Debug.LogWarning("SettingPanel�� ������� �ʾҽ��ϴ�.");
        }
    }
}
