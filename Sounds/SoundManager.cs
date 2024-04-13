using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public float MusicVolume {  get; private set; }               //背景音乐的音量
    public float SfxVolume { get; private set; }        //音效音量


    AudioSource m_MusicSource;  //用于BGM
    AudioSource m_SfxSource;    //用于音效

    Dictionary<string, AudioClip> m_AudioDict;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    
        //初始化（添加两个音频源）
        m_MusicSource = gameObject.AddComponent<AudioSource>();
        m_SfxSource = gameObject.AddComponent<AudioSource>();

        m_AudioDict = new Dictionary<string, AudioClip>();
    }

    private void Start()
    {
        //初始化音量
        MusicVolume = 1f;
        SfxVolume = 1f;
    }








    //使用Addressables加载音频
    public async Task<AudioClip> LoadClipAsync(string name)
    {
        //如果字典里已经有了，则直接返回
        if (m_AudioDict.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }


        //字典里没有的话就异步加载
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


    public void ReleaseClip()
    {

    }



    //公共函数，用于外部调用播放BGM
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

    //内部函数
    private void ConfigureAndPlayBGM(AudioClip thisClip, bool isLoop, float thisVolume)
    {
        m_MusicSource.Stop();
        m_MusicSource.clip = thisClip;
        m_MusicSource.loop = isLoop;
        m_MusicSource.volume = MusicVolume * thisVolume;
        m_MusicSource.Play();
    }











    //公共函数，用于外部调用播放音效
    public async void PlaySFXAsync(string clipName, float thisVolume = 1f)     //播放武器攻击音效
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

    public async void PlaySFXAsyncWithAudioSource(AudioSource thisAudioSource, string clipName, float thisVolume = 1f)     //播放武器攻击音效
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



    //内部函数
    private void ConfigureAndPlaySFX(AudioClip thisClip, float thisVolume)
    {
        if (thisClip != null)
        {
            //仅播放一次音效，使用参数中的音量
            m_SfxSource.PlayOneShot(thisClip, SfxVolume * thisVolume);
        }
    }

    //该函数的参数中传递音频源，用于类似3D的播放，形成近大远小的效果
    private void ConfigureAndPlaySFXWithAudioSource(AudioSource thisAudioSource, AudioClip thisClip, float thisVolume)
    {
        if (thisClip != null)
        {
            //仅播放一次音效，使用参数中的音量
            thisAudioSource.PlayOneShot(thisClip, SfxVolume * thisVolume);
        }
    }
}
