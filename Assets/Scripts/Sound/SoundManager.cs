using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer; // 인스펙터에서 연결 (Mixer에 'BGSoundVolume' 파라미터 있어야 함)

    [Header("BGM & 대상 오브젝트")]
    public AudioSource bgSound; // 인스펙터 또는 코드에서 자동 생성
    public AudioClip[] bglist;  // BGM 목록
    public GameObject[] bgObjects; // BGM과 대응되는 활성 오브젝트들

    private AudioSource sfxSource; // 효과음 재생용
    private bool isPlaying = false;
    private AudioClip lastRequestedClip = null;
    private AudioClip currentBGM = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // bgSound 없으면 자동 생성
            if (bgSound == null)
            {
                GameObject bgObj = new GameObject("BGSound");
                bgObj.transform.SetParent(this.transform);
                bgSound = bgObj.AddComponent<AudioSource>();
                bgSound.loop = true;
            }

            // sfxSource도 생성
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;

            // Mixer 연결
            if (mixer != null)
            {
                var bgGroup = mixer.FindMatchingGroups("BGSound");
                if (bgGroup.Length > 0)
                    bgSound.outputAudioMixerGroup = bgGroup[0];

                var sfxGroup = mixer.FindMatchingGroups("SFX");
                if (sfxGroup.Length > 0)
                    sfxSource.outputAudioMixerGroup = sfxGroup[0];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckAndPlayBgSound();
    }

    public void BGSoundVolume(float val)
    {
        Debug.Log("볼륨 조절 호출됨: " + val); // 로그 찍힘 확인

        if (mixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20;
            mixer.SetFloat("BGSoundVolume", dB);
            Debug.Log("볼륨(dB): " + dB); // 실제 적용 값 확인
        }
        else
        {
            Debug.LogWarning("Mixer가 연결되어 있지 않습니다.");
        }
    }


    public void SFXPlay(string sfxName, AudioClip clip)
    {
        if (clip == null) return;

        if (isPlaying)
        {
            lastRequestedClip = clip;
        }
        else
        {
            StartCoroutine(PlaySound(clip));
        }
    }

    private IEnumerator PlaySound(AudioClip clip)
    {
        isPlaying = true;
        sfxSource.clip = clip;
        sfxSource.Play();
        yield return new WaitForSeconds(clip.length);
        isPlaying = false;

        if (lastRequestedClip != null)
        {
            AudioClip next = lastRequestedClip;
            lastRequestedClip = null;
            StartCoroutine(PlaySound(next));
        }
    }

    public void BgSoundPlay(AudioClip clip)
    {
        if (clip != null && clip != currentBGM)
        {
            currentBGM = clip;
            bgSound.clip = clip;
            bgSound.Play();
        }
    }

    public void CheckAndPlayBgSound()
    {
        for (int i = 0; i < bgObjects.Length && i < bglist.Length; i++)
        {
            if (bgObjects[i] != null && bgObjects[i].activeSelf)
            {
                BgSoundPlay(bglist[i]);
                break;
            }
        }
    }
}
