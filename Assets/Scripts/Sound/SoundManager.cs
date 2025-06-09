using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer;

    [Header("BGM & 대상 오브젝트")]
    public AudioSource bgSound;
    public AudioClip[] bglist;
    public GameObject[] bgObjects;

    [SerializeField] private AudioClip introOneShot;
    private bool hasPlayedIntro = false;

    private AudioSource sfxSource;
    private bool isPlaying = false;
    private AudioClip lastRequestedClip;
    private AudioClip currentBGM = null;

    private bool hasPlayedHospitalFrontBGM = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgSound == null)
            {
                GameObject bgObj = new GameObject("BGSound");
                bgObj.transform.SetParent(this.transform);
                bgSound = bgObj.AddComponent<AudioSource>();
                bgSound.loop = true;
            }

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;

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
        // 1. intro 효과음 + BGM 병렬 재생
        if (bgObjects.Length > 6 && bgObjects[6] != null && bgObjects[6].activeSelf && !hasPlayedIntro)
        {
            hasPlayedIntro = true;
            StartCoroutine(PlayIntroThenBGM_Parallel());
        }

        // 2. 병원 앞 활성화 → bglist[7] 재생
        if (!hasPlayedHospitalFrontBGM && bgObjects.Length > 7 && bgObjects[7] != null)
        {
            if (bgObjects[7].activeSelf && bglist.Length > 7 && bglist[7] != null)
            {
                BgSoundPlay(bglist[7]);
                hasPlayedHospitalFrontBGM = true;
                Debug.Log("[SoundManager] 병원 앞 BGM 재생됨: " + bglist[7].name);
            }
        }
        // bgObjects[6]이 비활성화되고, bgObjects[7]이 활성화된 경우 → bglist[1] 재생
        if (bgObjects.Length > 7 && bgObjects[6] != null && bgObjects[7] != null)
        {
            if (!bgObjects[6].activeSelf && bgObjects[7].activeSelf)
            {
                if (bglist.Length > 1 && bglist[1] != null && currentBGM != bglist[1])
                {
                    BgSoundPlay(bglist[1]);
                    Debug.Log("[SoundManager] bgObjects[6] OFF + bgObjects[7] ON → bglist[1] 재생됨: " + bglist[1].name);
                }
            }
        }
    }

    private IEnumerator PlayIntroThenBGM_Parallel()
    {
        if (introOneShot != null)
        {
            sfxSource.clip = introOneShot;
            sfxSource.volume = 1f;
            sfxSource.Play();

            CheckAndPlayBgSound();

            yield return new WaitForSeconds(5f);
            yield return StartCoroutine(FadeOutAudio(sfxSource, 1f));
            sfxSource.Stop();
            sfxSource.clip = null;
        }
        else
        {
            CheckAndPlayBgSound();
        }
    }

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

    public void BGSoundVolume(float val)
    {
        if (mixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20;
            mixer.SetFloat("BGSoundVolume", dB);
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
