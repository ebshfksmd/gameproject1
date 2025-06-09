using UnityEngine;

public class KeyObjectActivator : MonoBehaviour
{
    [Header("활성화할 오브젝트들 (11개 순서대로 설정)")]
    [SerializeField] private GameObject[] keyObjects = new GameObject[11];

    void Update()
    {
        // 이동 (←, A)
        CheckKey(KeyCode.LeftArrow, 0);
        CheckKey(KeyCode.A, 0);

        // 이동 (→, D)
        CheckKey(KeyCode.RightArrow, 1);
        CheckKey(KeyCode.D, 1);

        // 점프
        CheckKey(KeyCode.Space, 2);

        // 전환
        CheckKey(KeyCode.Tab, 3);

        // 달리기
        CheckKey(KeyCode.LeftShift, 4);

        // 공격
        CheckKey(KeyCode.F, 5);

        // 스킬
        CheckKey(KeyCode.Q, 6);
        CheckKey(KeyCode.W, 7);
        CheckKey(KeyCode.E, 8);
        CheckKey(KeyCode.R, 9);

        // 닫기
        CheckKey(KeyCode.Escape, 10);
    }

    private void CheckKey(KeyCode key, int index)
    {
        if (keyObjects == null || index < 0 || index >= keyObjects.Length) return;

        if (Input.GetKeyDown(key))
        {
            if (keyObjects[index] != null)
                keyObjects[index].SetActive(true);
        }

        if (Input.GetKeyUp(key))
        {
            if (keyObjects[index] != null)
                keyObjects[index].SetActive(false);
        }
    }
}
