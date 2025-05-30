using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

/// <summary>
/// ���� �� BGM �� ȿ������ �����ϴ� �̱��� ���� �Ŵ���
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer; // Mixer�� 'BGSoundVolume'�� 'SFX' �׷� �ʿ�

    [Header("BGM & ��� ������Ʈ")]
    public AudioSource bgSound;          // BGM ����� AudioSource
    public AudioClip[] bglist;           // ��Ȳ�� BGM ���
    public GameObject[] bgObjects;       // �� BGM�� �����ϴ� ������Ʈ�� (Ȱ�� ���� ����)

    [SerializeField] private AudioClip introOneShot; // �ʹ� ȿ���� Ŭ�� (���̵� �ƿ� ����)
    private bool hasPlayedIntro = false;             // introOneShot ��� ����

    private AudioSource sfxSource;         // ȿ���� ���� AudioSource
    private bool isPlaying = false;        // ȿ���� ��� �� ����
    private AudioClip lastRequestedClip;   // ť�� �ִ� ������ ��û�� ȿ����
    private AudioClip currentBGM = null;   // ���� ��� ���� BGM

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM AudioSource ������ �ڵ� ����
            if (bgSound == null)
            {
                GameObject bgObj = new GameObject("BGSound");
                bgObj.transform.SetParent(this.transform);
                bgSound = bgObj.AddComponent<AudioSource>();
                bgSound.loop = true;
            }

            // SFX�� AudioSource ����
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;

            // AudioMixer ����
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
        // ����: bgObjects[6]�� Ȱ��ȭ�ǰ� intro�� ���� ������� �ʾҴٸ� ����
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

            // BGM�� �Բ� ��� (�ٷ�)
            CheckAndPlayBgSound();

            // 5�� ���� ��ٸ� �� ���̵� �ƿ�
            yield return new WaitForSeconds(5f);
            yield return StartCoroutine(FadeOutAudio(sfxSource, 1f)); // 1�� ���̵�ƿ�
            sfxSource.Stop();
            sfxSource.clip = null;
        }
        else
        {
            CheckAndPlayBgSound(); // ȿ���� ���� ���� �׳� BGM ����
        }
    }

    /// <summary>
    /// ȿ������ ����� �� ���̵� �ƿ���Ű�� BGM ���
    /// </summary>
    private IEnumerator PlayIntroThenBGM()
    {
        if (introOneShot != null)
        {
            sfxSource.clip = introOneShot;
            sfxSource.volume = 1f;
            sfxSource.Play();

            yield return new WaitForSeconds(5f); // ȿ���� 5�� ���� ����

            yield return StartCoroutine(FadeOutAudio(sfxSource, 1f)); // 1�ʰ� ���̵�ƿ�
            sfxSource.Stop();
            sfxSource.clip = null;
        }

        CheckAndPlayBgSound(); // ���� BGM ���
    }

    /// <summary>
    /// AudioSource�� ���̵�ƿ�
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
    /// BGM ���� ���� �Լ� (0.0001 ~ 1.0 �� dB�� ��ȯ)
    /// </summary>
    public void BGSoundVolume(float val)
    {
        Debug.Log("���� ���� ȣ���: " + val);

        if (mixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20;
            mixer.SetFloat("BGSoundVolume", dB);
            Debug.Log("����(dB): " + dB);
        }
        else
        {
            Debug.LogWarning("Mixer�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    /// <summary>
    /// ȿ���� ��û
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
    /// ȿ���� ��� �ڷ�ƾ
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
    /// Ư�� BGM�� ���
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
    /// bgObjects�� Ȯ���Ͽ� Ȱ��ȭ�� ���� BGM ���
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
