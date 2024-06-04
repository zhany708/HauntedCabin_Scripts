using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;



public class SoundManager : ManagerTemplate<SoundManager>
{
    public SO_AudioClipKeys AudioClipKeys;

    public float MusicVolume { get; private set; }               //背景音乐的音量
    public float SfxVolume { get; private set; }        //音效音量



    AudioSource m_MusicSource;  //用于BGM的播放器
    AudioSource m_SfxSource;    //用于音效的播放器

    //由于音频管理器加载的文件类型是音频，不是游戏物体，所以需要有自己的字典
    Dictionary<string, AudioClip> m_AudioDict;





    #region Unity内部函数循环
    protected override void Awake()
    {
        base.Awake();


        //初始化（添加两个音频源）
        m_MusicSource = gameObject.AddComponent<AudioSource>();
        m_SfxSource = gameObject.AddComponent<AudioSource>();

        m_AudioDict = new Dictionary<string, AudioClip>();


        //初始化音量
        MusicVolume = 1f;
        SfxVolume = 1f;
    }
    #endregion


    #region 资源加载相关
    //使用Addressables加载音频
    private async Task<AudioClip> LoadClipAsync(string name)
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


    //在Addressables里释放音频，只有这样才能释放内存
    public void ReleaseAudioClip(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
            key = key.Substring(0, key.Length - 7);
        }


        if (m_AudioDict.TryGetValue(key, out AudioClip clip))
        {
            Addressables.Release(clip);

            //从字典中移除音频
            m_AudioDict.Remove(key);

            //Debug.Log("AudioClip released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This AudioClip is not loaded yet, cannot release: " + key);
        }
    }
    #endregion


    #region BGM相关
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
    #endregion


    #region 音效相关
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
            m_SfxSource.PlayOneShot(thisClip, SfxVolume * thisVolume * RandomVolume() );
        }
    }

    //该函数的参数中传递音频源，用于类似3D的播放，形成近大远小的效果
    private void ConfigureAndPlaySFXWithAudioSource(AudioSource thisAudioSource, AudioClip thisClip, float thisVolume)
    {
        if (thisClip != null)
        {
            //仅播放一次音效，使用参数中的音量
            thisAudioSource.PlayOneShot(thisClip, SfxVolume * thisVolume * RandomVolume() );
        }
    }


    //随机音高（98%-103%），这样不会显得音效单调
    private float RandomVolume()
    {
        return UnityEngine.Random.Range(0.98f, 1.03f);
    }
    #endregion
}