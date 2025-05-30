using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/// <summary>
/// 게임 내 BGM 및 효과음을 관리하는 싱글턴 사운드 매니저
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer; // Mixer에 'BGSoundVolume'과 'SFX' 그룹 필요

    [Header("BGM & 대상 오브젝트")]
    public AudioSource bgSound;          // BGM 재생용 AudioSource
    public AudioClip[] bglist;           // 상황별 BGM 목록
    public GameObject[] bgObjects;       // 각 BGM에 대응하는 오브젝트들 (활성 상태 기준)

    [SerializeField] private AudioClip introOneShot; // 초반 효과음 클립 (페이드 아웃 포함)
    private bool hasPlayedIntro = false;             // introOneShot 재생 여부

    private AudioSource sfxSource;         // 효과음 전용 AudioSource
    private bool isPlaying = false;        // 효과음 재생 중 여부
    private AudioClip lastRequestedClip;   // 큐에 있는 마지막 요청된 효과음
    private AudioClip currentBGM = null;   // 현재 재생 중인 BGM

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM AudioSource 없으면 자동 생성
            if (bgSound == null)
            {
                GameObject bgObj = new GameObject("BGSound");
                bgObj.transform.SetParent(this.transform);
                bgSound = bgObj.AddComponent<AudioSource>();
                bgSound.loop = true;
            }

            // SFX용 AudioSource 생성
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;

            // AudioMixer 연결
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
        // 조건: bgObjects[6]이 활성화되고 intro가 아직 재생되지 않았다면 실행
        if (bgObjects.Length > 6 && bgObjects[6] != null && bgObjects[6].activeSelf && !hasPlayedIntro)
        {
            hasPlayedIntro = true;
            StartCoroutine(PlayIntroThenBGM_Parallel());
        }
        else if (!hasPlayedIntro)
        {
            CheckAndPlayBgSound();
        }
    }

    private IEnumerator PlayIntroThenBGM_Parallel()
    {
        if (introOneShot != null)
        {
            sfxSource.clip = introOneShot;
            sfxSource.volume = 1f;
            sfxSource.Play();

            // BGM도 함께 재생 (바로)
            CheckAndPlayBgSound();

            // 5초 동안 기다린 뒤 페이드 아웃
            yield return new WaitForSeconds(5f);
            yield return StartCoroutine(FadeOutAudio(sfxSource, 1f)); // 1초 페이드아웃
            sfxSource.Stop();
            sfxSource.clip = null;
        }
        else
        {
            CheckAndPlayBgSound(); // 효과음 없을 때는 그냥 BGM 실행
        }
    }

    /// <summary>
    /// 효과음을 재생한 후 페이드 아웃시키고 BGM 재생
    /// </summary>
    private IEnumerator PlayIntroThenBGM()
    {
        if (introOneShot != null)
        {
            sfxSource.clip = introOneShot;
            sfxSource.volume = 1f;
            sfxSource.Play();

            yield return new WaitForSeconds(5f); // 효과음 5초 동안 유지

            yield return StartCoroutine(FadeOutAudio(sfxSource, 1f)); // 1초간 페이드아웃
            sfxSource.Stop();
            sfxSource.clip = null;
        }

        CheckAndPlayBgSound(); // 이후 BGM 재생
    }

    /// <summary>
    /// AudioSource를 페이드아웃
    /// </summary>
    private IEnumerator FadeOutAudio(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.volume = 0f;
    }

    /// <summary>
    /// BGM 볼륨 설정 함수 (0.0001 ~ 1.0 → dB로 변환)
    /// </summary>
    public void BGSoundVolume(float val)
    {
        Debug.Log("볼륨 조절 호출됨: " + val);

        if (mixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20;
            mixer.SetFloat("BGSoundVolume", dB);
            Debug.Log("볼륨(dB): " + dB);
        }
        else
        {
            Debug.LogWarning("Mixer가 연결되어 있지 않습니다.");
        }
    }

    /// <summary>
    /// 효과음 요청
    /// </summary>
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

    /// <summary>
    /// 효과음 재생 코루틴
    /// </summary>
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

    /// <summary>
    /// 특정 BGM을 재생
    /// </summary>
    public void BgSoundPlay(AudioClip clip)
    {
        if (clip != null && clip != currentBGM)
        {
            currentBGM = clip;
            bgSound.clip = clip;
            bgSound.Play();
        }
    }

    /// <summary>
    /// bgObjects를 확인하여 활성화된 것의 BGM 재생
    /// </summary>
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
