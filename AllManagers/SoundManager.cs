using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;



public class SoundManager : ManagerTemplate<SoundManager>
{
    public SO_AudioClipKeys AudioClipKeys;

    public float MusicVolume { get; private set; } = 0.8f;      //背景音乐的音量
    public float SfxVolume { get; private set; } = 1f;        //音效音量



    AudioSource m_MusicSource;  //用于BGM的播放器
    AudioSource m_SfxSource;    //用于音效的播放器

    //由于音频管理器加载的文件类型是音频，不是游戏物体，所以需要有自己的字典
    public Dictionary<string, AudioClip> AudioDict { get; private set; } = new Dictionary<string, AudioClip>();





    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();


        //初始化（添加两个音频源）
        m_MusicSource = gameObject.AddComponent<AudioSource>();
        m_SfxSource = gameObject.AddComponent<AudioSource>();
    }
    #endregion


    #region 资源加载相关
    //使用Addressables加载音频
    public async Task<AudioClip> LoadClipAsync(string name)
    {
        //如果字典里已经有了，则直接返回
        if (AudioDict.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }


        //字典里没有的话就异步加载
        AudioClip loadedClip = await Addressables.LoadAssetAsync<AudioClip>(name).Task;
        if (loadedClip != null)
        {
            AudioDict[name] = loadedClip;
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


        if (AudioDict.TryGetValue(key, out AudioClip clip))
        {
            Addressables.Release(clip);

            //从字典中移除音频
            AudioDict.Remove(key);

            //Debug.Log("AudioClip released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This AudioClip is not loaded yet, cannot release: " + key);
            return;
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
    //公共函数，用于外部调用播放音效（传递string，随后异步加载）
    public async void PlaySFXAsync(string clipName, float thisVolume, bool isLoop = false)
    {
        try
        {
            AudioClip clip = await LoadClipAsync(clipName);
            if (clip != null)
            {
                ConfigureAndPlaySFX(clip, thisVolume, isLoop);
            }

            else
            {
                Debug.LogError("Failed to load audio clip: " + clipName);
                return;
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error playing music: " + ex.Message);
            return;
        }
    }

    //传递音效片段，直接播放无需异步加载
    public void PlaySFX(AudioClip clip, float thisVolume, bool isLoop = false)
    {    
        ConfigureAndPlaySFX(clip, thisVolume, isLoop);           
    }


    //该函数的参数中传递音频源，用于类似3D的播放，形成近大远小的效果
    public async void PlaySFXAsyncWithAudioSource(AudioSource thisAudioSource, string clipName, float thisVolume, bool isLoop = false)
    {
        try
        {
            AudioClip clip = await LoadClipAsync(clipName);
            if (clip != null)
            {
                //调用内部函数以播放音效
                ConfigureAndPlaySFXWithAudioSource(thisAudioSource, clip, thisVolume, isLoop);
            }

            else
            {
                Debug.LogError("Failed to load audio clip: " + clipName);
                return;
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error playing music: " + ex.Message);
            return;
        }
    }



    //内部函数
    private void ConfigureAndPlaySFX(AudioClip thisClip, float thisVolume, bool isLoop = false)
    {
        if (thisClip != null)
        {
            //检查要播放的音效是否循环
            if (isLoop)
            {
                m_SfxSource.Stop();                             //停止播放器现在正在播放的所有音效（无论是用哪种方式播放）
                m_SfxSource.clip = thisClip;
                m_SfxSource.loop = isLoop;
                m_SfxSource.volume = SfxVolume * thisVolume;
                m_SfxSource.Play();
            }

            else
            {
                //仅播放一次音效，使用参数中的音量
                m_SfxSource.PlayOneShot(thisClip, SfxVolume * thisVolume * RandomVolume() );
            }           
        }
    }

    //该函数的参数中传递音频源，用于类似3D的播放，形成近大远小的效果
    private void ConfigureAndPlaySFXWithAudioSource(AudioSource thisAudioSource, AudioClip thisClip, float thisVolume, bool isLoop = false)
    {
        if (thisClip != null)
        {
            //检查要播放的音效是否循环
            if (isLoop)
            {
                thisAudioSource.Stop();
                thisAudioSource.clip = thisClip;
                thisAudioSource.loop = isLoop;
                thisAudioSource.volume = SfxVolume * thisVolume;
                thisAudioSource.Play();
            }

            else
            {
                //仅播放一次音效，使用参数中的音量
                thisAudioSource.PlayOneShot(thisClip, SfxVolume * thisVolume * RandomVolume() );
            }                      
        }
    }
    #endregion


    #region 关闭音效
    public void StopAudioPlay(bool isBGM)
    {
        if (isBGM)
        {
            m_MusicSource.Stop();
        }
        else
        {
            m_SfxSource.Stop();
        }
    }

    public void PauseOrResume(bool isBGM)
    {
        if (isBGM)
        {
            //如果该播放器正在播放，则暂停
            if (m_MusicSource.isPlaying)
            {
                m_MusicSource.Pause();
            }
            else
            {
                m_MusicSource.UnPause();
            }
        }

        else
        {
            if (m_SfxSource.isPlaying)
            {
                m_SfxSource.Pause();
            }
            else
            {
                m_SfxSource.UnPause();
            }
        }
    }
    #endregion


    #region 其余函数
    //随机音高（98%-103%），这样不会显得音效单调
    private float RandomVolume()
    {
        return UnityEngine.Random.Range(0.98f, 1.03f);
    }
    #endregion
}