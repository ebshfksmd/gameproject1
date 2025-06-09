using UnityEngine;

public class KeyObjectActivator : MonoBehaviour
{
    [Header("Ȱ��ȭ�� ������Ʈ�� (11�� ������� ����)")]
    [SerializeField] private GameObject[] keyObjects = new GameObject[11];

    void Update()
    {
        // �̵� (��, A)
        CheckKey(KeyCode.LeftArrow, 0);
        CheckKey(KeyCode.A, 0);

        // �̵� (��, D)
        CheckKey(KeyCode.RightArrow, 1);
        CheckKey(KeyCode.D, 1);

        // ����
        CheckKey(KeyCode.Space, 2);

        // ��ȯ
        CheckKey(KeyCode.Tab, 3);

        // �޸���
        CheckKey(KeyCode.LeftShift, 4);

        // ����
        CheckKey(KeyCode.F, 5);

        // ��ų
        CheckKey(KeyCode.Q, 6);
        CheckKey(KeyCode.W, 7);
        CheckKey(KeyCode.E, 8);
        CheckKey(KeyCode.R, 9);

        // �ݱ�
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
