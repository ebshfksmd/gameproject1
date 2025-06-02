using UnityEngine;

public class HospitalTransitionTrigger : MonoBehaviour
{
    public GameObject hospitalFrontObject;

    private void Start()
    {
        // ���� ���� ������Ʈ Ȱ��ȭ
        if (hospitalFrontObject != null)
        {
            hospitalFrontObject.SetActive(true);

            // Ȱ��ȭ ���� ����� üũ ��û
            if (SoundManager.instance != null)
            {
                SoundManager.instance.CheckAndPlayBgSound();
            }
            else
            {
                Debug.LogWarning("SoundManager.instance�� �������� �ʽ��ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("hospitalFrontObject�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
