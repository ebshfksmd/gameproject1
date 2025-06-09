using UnityEngine;

public class DialogueTriggerOnPlayer : MonoBehaviour
{
    [Tooltip("�浹 �� Ȱ��ȭ�� ��� ������Ʈ")]
    [SerializeField] private GameObject targetDialogueObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �÷��̾� �±װ� ���� ������Ʈ�� �浹�� ���
        if (other.CompareTag("Player"))
        {
            Debug.Log("[DialogueTriggerOnPlayer] �÷��̾�� �浹��");

            // �ڽ��� ��Ȱ��ȭ
            gameObject.SetActive(false);

            // ��� ������Ʈ Ȱ��ȭ
            if (targetDialogueObject != null)
            {
                targetDialogueObject.SetActive(true);
                Debug.Log("[DialogueTriggerOnPlayer] ��� ������Ʈ Ȱ��ȭ��: " + targetDialogueObject.name);
            }
        }
    }
}
