using UnityEngine;

public class DialogueTriggerOnPlayer : MonoBehaviour
{
    [Tooltip("충돌 시 활성화할 대사 오브젝트")]
    [SerializeField] private GameObject targetDialogueObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그가 붙은 오브젝트와 충돌한 경우
        if (other.CompareTag("Player"))
        {
            Debug.Log("[DialogueTriggerOnPlayer] 플레이어와 충돌됨");

            // 자신은 비활성화
            gameObject.SetActive(false);

            // 대사 오브젝트 활성화
            if (targetDialogueObject != null)
            {
                targetDialogueObject.SetActive(true);
                Debug.Log("[DialogueTriggerOnPlayer] 대사 오브젝트 활성화됨: " + targetDialogueObject.name);
            }
        }
    }
}
