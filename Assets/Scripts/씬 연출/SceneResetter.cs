using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResetManager : MonoBehaviour
{
    [Header("리셋 대상 후보 오브젝트들 (5개)")]
    [SerializeField] private GameObject[] candidates;

    [Header("메인화면 오브젝트")]
    [SerializeField] private GameObject mainScreenObject;

    // 정적 저장 (씬 리로드 후에도 유지)
    private static int savedIndex = -1;
    private static bool isRestoring = false;

    void Start()
    {
        Debug.Log("[SceneResetManager] Start 호출됨");
        Debug.Log($"[SceneResetManager] isRestoring: {isRestoring}, savedIndex: {savedIndex}");

        if (isRestoring && savedIndex >= 0 && savedIndex < candidates.Length)
        {
            Debug.Log($"[SceneResetManager] 복원 모드 시작 - index: {savedIndex}");

            candidates[savedIndex].SetActive(true);

            if (mainScreenObject != null)
                mainScreenObject.SetActive(false);
        }

        // 상태 초기화 (다시 누르기 전까지 무효)
        isRestoring = false;
        savedIndex = -1;
    }

    // ▶ 버튼 A: 완전 초기화
    public void ResetCompletely()
    {
        Debug.Log("[SceneResetManager] ResetCompletely 버튼 클릭됨");

        savedIndex = -1;
        isRestoring = false;
        ReloadScene();
    }

    // ▶ 버튼 B: 현재 활성화된 후보를 저장하고 복원 모드로 리셋
    public void ResetAndRestoreActiveObject()
    {
        Debug.Log("[SceneResetManager] ResetAndRestoreActiveObject 버튼 클릭됨");

        savedIndex = -1;
        for (int i = 0; i < candidates.Length; i++)
        {
            if (candidates[i] != null && candidates[i].activeSelf)
            {
                savedIndex = i;
                isRestoring = true;
                Debug.Log($"[SceneResetManager] 저장된 index: {savedIndex}");
                break;
            }
        }

        ReloadScene();
    }

    private void ReloadScene()
    {
        Scene current = SceneManager.GetActiveScene();
        Debug.Log($"[SceneResetManager] ReloadScene 호출 - {current.name}");
        SceneManager.LoadScene(current.name);
    }

    // 디터에서 강제 테스트용 (우클릭 메뉴)
    [ContextMenu("강제로 첫 번째 후보 저장")]
    private void ForceSaveForTest()
    {
        savedIndex = 0;
        isRestoring = true;
        Debug.Log("[SceneResetManager] 강제 저장 - index 0");
    }
}
