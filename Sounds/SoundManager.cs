using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;


public class SoundManager : ManagerTemplate<SoundManager>
{
    public SO_AudioClipKeys AudioClipKeys;

    public float MusicVolume { get; private set; }               //�������ֵ�����
    public float SfxVolume { get; private set; }        //��Ч����



    AudioSource m_MusicSource;  //����BGM
    AudioSource m_SfxSource;    //������Ч

    //������Ƶ���������ص��ļ���������Ƶ��������Ϸ���壬������Ҫ���Լ����ֵ�
    Dictionary<string, AudioClip> m_AudioDict;






    protected override void Awake()
    {
        base.Awake();


        //��ʼ�������������ƵԴ��
        m_MusicSource = gameObject.AddComponent<AudioSource>();
        m_SfxSource = gameObject.AddComponent<AudioSource>();

        m_AudioDict = new Dictionary<string, AudioClip>();


        //��ʼ������
        MusicVolume = 1f;
        SfxVolume = 1f;
    }






    //ʹ��Addressables������Ƶ
    private async Task<AudioClip> LoadClipAsync(string name)
    {
        //����ֵ����Ѿ����ˣ���ֱ�ӷ���
        if (m_AudioDict.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }


        //�ֵ���û�еĻ����첽����
        AudioClip loadedClip = await Addressables.LoadAssetAsync<AudioClip>(name).Task;
        if (loadedClip != null)
        {
            m_AudioDict[name] = loadedClip;
        }

        else
        {
            Debug.LogError("Failed to load audio clip: " + name);
        }

        return loadedClip;
    }


    //��Addressables���ͷ���Ƶ��ֻ�����������ͷ��ڴ�
    public void ReleaseAudioClip(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
            key = key.Substring(0, key.Length - 7);
        }


        if (m_AudioDict.TryGetValue(key, out AudioClip clip))
        {
            Addressables.Release(clip);

            //���ֵ����Ƴ���Ƶ
            m_AudioDict.Remove(key);

            //Debug.Log("AudioClip released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This AudioClip is not loaded yet, cannot release: " + key);
        }
    }



    //���������������ⲿ���ò���BGM
    public async Task PlayBGMAsync(string clipName, bool isLoop, float thisVolume = 1f)
    {
        try
        {
            AudioClip clip = await LoadClipAsync(clipName);
            if (clip != null)
            {
                ConfigureAndPlayBGM(clip, isLoop, thisVolume);
            }

            else
            {
                Debug.LogError("Failed to load audio clip: " + clipName);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error playing music: " + ex.Message);
        }
    }

    //�ڲ�����
    private void ConfigureAndPlayBGM(AudioClip thisClip, bool isLoop, float thisVolume)
    {
        m_MusicSource.Stop();
        m_MusicSource.clip = thisClip;
        m_MusicSource.loop = isLoop;
        m_MusicSource.volume = MusicVolume * thisVolume;
        m_MusicSource.Play();
    }











    //���������������ⲿ���ò�����Ч
    public async void PlaySFXAsync(string clipName, float thisVolume = 1f)     //��������������Ч
    {
        try
        {
            AudioClip clip = await LoadClipAsync(clipName);
            if (clip != null)
            {
                ConfigureAndPlaySFX(clip, thisVolume);
            }

            else
            {
                Debug.LogError("Failed to load audio clip: " + clipName);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error playing music: " + ex.Message);
        }
    }

    public async void PlaySFXAsyncWithAudioSource(AudioSource thisAudioSource, string clipName, float thisVolume = 1f)     //��������������Ч
    {
        try
        {
            AudioClip clip = await LoadClipAsync(clipName);
            if (clip != null)
            {
                ConfigureAndPlaySFXWithAudioSource(thisAudioSource, clip, thisVolume);
            }

            else
            {
                Debug.LogError("Failed to load audio clip: " + clipName);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error playing music: " + ex.Message);
        }
    }



    //�ڲ�����
    private void ConfigureAndPlaySFX(AudioClip thisClip, float thisVolume)
    {
        if (thisClip != null)
        {
            //������һ����Ч��ʹ�ò����е�����
            m_SfxSource.PlayOneShot(thisClip, SfxVolume * thisVolume * RandomVolume() );
        }
    }

    //�ú����Ĳ����д�����ƵԴ����������3D�Ĳ��ţ��γɽ���ԶС��Ч��
    private void ConfigureAndPlaySFXWithAudioSource(AudioSource thisAudioSource, AudioClip thisClip, float thisVolume)
    {
        if (thisClip != null)
        {
            //������һ����Ч��ʹ�ò����е�����
            thisAudioSource.PlayOneShot(thisClip, SfxVolume * thisVolume * RandomVolume() );
        }
    }


    //������ߣ�98%-103%�������������Ե���Ч����
    private float RandomVolume()
    {
        return UnityEngine.Random.Range(0.98f, 1.03f);
    }
}