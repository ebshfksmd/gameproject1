using UnityEngine;

public class HospitalTransitionTrigger : MonoBehaviour
{
    public GameObject hospitalFrontObject;

    private void Start()
    {
        // 병원 전경 오브젝트 활성화
        if (hospitalFrontObject != null)
        {
            hospitalFrontObject.SetActive(true);

            // 활성화 직후 배경음 체크 요청
            if (SoundManager.instance != null)
            {
                SoundManager.instance.CheckAndPlayBgSound();
            }
            else
            {
                Debug.LogWarning("SoundManager.instance가 존재하지 않습니다.");
            }
        }
        else
        {
            Debug.LogWarning("hospitalFrontObject가 할당되지 않았습니다.");
        }
    }
}
