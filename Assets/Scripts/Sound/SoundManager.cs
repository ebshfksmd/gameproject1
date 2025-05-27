using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Mixer")]
    public AudioMixer mixer; // �ν����Ϳ��� ���� (Mixer�� 'BGSoundVolume' �Ķ���� �־�� ��)

    [Header("BGM & ��� ������Ʈ")]
    public AudioSource bgSound; // �ν����� �Ǵ� �ڵ忡�� �ڵ� ����
    public AudioClip[] bglist;  // BGM ���
    public GameObject[] bgObjects; // BGM�� �����Ǵ� Ȱ�� ������Ʈ��

    private AudioSource sfxSource; // ȿ���� �����
    private bool isPlaying = false;
    private AudioClip lastRequestedClip = null;
    private AudioClip currentBGM = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // bgSound ������ �ڵ� ����
            if (bgSound == null)
            {
                GameObject bgObj = new GameObject("BGSound");
                bgObj.transform.SetParent(this.transform);
                bgSound = bgObj.AddComponent<AudioSource>();
                bgSound.loop = true;
            }

            // sfxSource�� ����
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;

            // Mixer ����
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
        Debug.Log("���� ���� ȣ���: " + val); // �α� ���� Ȯ��

        if (mixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(val, 0.0001f, 1f)) * 20;
            mixer.SetFloat("BGSoundVolume", dB);
            Debug.Log("����(dB): " + dB); // ���� ���� �� Ȯ��
        }
        else
        {
            Debug.LogWarning("Mixer�� ����Ǿ� ���� �ʽ��ϴ�.");
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
