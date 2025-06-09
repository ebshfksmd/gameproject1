using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetManager : MonoBehaviour
{
    [Header("���� ��� �ĺ� ������Ʈ�� (5��)")]
    [SerializeField] private GameObject[] candidates;

    [Header("����ȭ�� ������Ʈ")]
    [SerializeField] private GameObject mainScreenObject;

    // ���� ���� (�� ���ε� �Ŀ��� ����)
    private static int savedIndex = -1;
    private static bool isRestoring = false;

    void Start()
    {
        Debug.Log("[SceneResetManager] Start ȣ���");
        Debug.Log($"[SceneResetManager] isRestoring: {isRestoring}, savedIndex: {savedIndex}");

        if (isRestoring && savedIndex >= 0 && savedIndex < candidates.Length)
        {
            Debug.Log($"[SceneResetManager] ���� ��� ���� - index: {savedIndex}");

            candidates[savedIndex].SetActive(true);

            if (mainScreenObject != null)
                mainScreenObject.SetActive(false);
        }

        // ���� �ʱ�ȭ (�ٽ� ������ ������ ��ȿ)
        isRestoring = false;
        savedIndex = -1;
    }

    // �� ��ư A: ���� �ʱ�ȭ
    public void ResetCompletely()
    {
        Debug.Log("[SceneResetManager] ResetCompletely ��ư Ŭ����");

        savedIndex = -1;
        isRestoring = false;
        ReloadScene();
    }

    // �� ��ư B: ���� Ȱ��ȭ�� �ĺ��� �����ϰ� ���� ���� ����
    public void ResetAndRestoreActiveObject()
    {
        Debug.Log("[SceneResetManager] ResetAndRestoreActiveObject ��ư Ŭ����");

        savedIndex = -1;
        for (int i = 0; i < candidates.Length; i++)
        {
            if (candidates[i] != null && candidates[i].activeSelf)
            {
                savedIndex = i;
                isRestoring = true;
                Debug.Log($"[SceneResetManager] ����� index: {savedIndex}");
                break;
            }
        }

        ReloadScene();
    }

    private void ReloadScene()
    {
        Scene current = SceneManager.GetActiveScene();
        Debug.Log($"[SceneResetManager] ReloadScene ȣ�� - {current.name}");
        SceneManager.LoadScene(current.name);
    }

    // ���Ϳ��� ���� �׽�Ʈ�� (��Ŭ�� �޴�)
    [ContextMenu("������ ù ��° �ĺ� ����")]
    private void ForceSaveForTest()
    {
        savedIndex = 0;
        isRestoring = true;
        Debug.Log("[SceneResetManager] ���� ���� - index 0");
    }
}
