using UnityEngine;

public class SettingPanelToggle : MonoBehaviour
{
    [Header("����� ���� �г�")]
    [SerializeField] private GameObject settingPanel;

    void Update()
    {
        // ESC Ű�� ���� �г� ����
        if (settingPanel != null && settingPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettingPanel();
        }
    }

    /// <summary>
    /// ��ư Ŭ�� �� ȣ��: ���� �г��� �����
    /// </summary>
    public void ToggleSettingPanel()
    {
        if (settingPanel != null)
        {
            bool currentState = settingPanel.activeSelf;
            settingPanel.SetActive(!currentState);

            if (!currentState)
            {
                // ���� �г��� ���� �� ���� ����
                Time.timeScale = 0f;
            }
            else
            {
                // ���� �г��� ���� �� ���� �簳
                Time.timeScale = 1f;
            }
        }
        else
        {
            Debug.LogWarning("SettingPanel�� ������� �ʾҽ��ϴ�.");
        }
    }

    /// <summary>
    /// ESC Ű�� ���� �г��� �� �� ȣ���
    /// </summary>
    private void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
