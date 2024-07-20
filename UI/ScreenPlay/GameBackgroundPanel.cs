using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lean.Localization;




public class GameBackgroundPanel : BasePanel
{
    public TextMeshProUGUI FirstPartText;       //第一段文本
    public TextMeshProUGUI SecondPartText;      //第二段文本
    public TextMeshProUGUI LastPartText;        //最后一段文本
    public TextMeshProUGUI TipText;             //提示文本


    public string DoorKeyword;                  //大门关闭的字眼（LeanLocalization下的一个子物体的名字）


    public float KeyWordAudioVolume = 1.5f;     //所有字眼处音效的音量大小



    //储存需要播放音效的字眼（比如“关门”，“下雨”等）
    Dictionary<string, AudioClip> m_KeywordAudioDict = new Dictionary<string, AudioClip>();

    //用于检测音效是否已经播放过的字典（防止重复播放）
    Dictionary<AudioClip, bool> m_AudioCheckDict = new Dictionary<AudioClip, bool>();






    #region Unity内部函数
    protected async override void Awake()
    {
        if (FirstPartText == null || SecondPartText == null || LastPartText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the " + name);
            return;
        }

        if (DoorKeyword == "")
        {
            Debug.LogError("Some Lean Localization phrase keys are not written in the " + name);
            return;
        }

        await InitializeKeywordAudioDict();       //初始化字典
    }

    private void OnEnable()
    {
        //界面完全淡出后调用以下函数
        OnFadeOutFinished += ClosePanel;
        OnFadeOutFinished += PlayGame;

        OnFadeInFinished += StartTextAnimations;    //界面完全淡入后调用此函数
    }

    private async void Start() 
    {
        //提前加载一楼BGM
        await SoundManager.Instance.LoadClipAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment);

        //循环播放下雨音效
        SoundManager.Instance.PlaySFXAsync(SoundManager.Instance.AudioClipKeys.RainingKey, KeyWordAudioVolume, true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= PlayGame;

        OnFadeInFinished -= StartTextAnimations;
    }

    private void OnDestroy()
    {
        //释放本界面所有使用过的的音效     
        SoundManager.Instance.ReleaseAudioClip(SoundManager.Instance.AudioClipKeys.RainingKey);
        SoundManager.Instance.ReleaseAudioClip(SoundManager.Instance.AudioClipKeys.MainDoorCloseKey);
    }
    #endregion


    #region 初始化
    private async Task InitializeKeywordAudioDict()
    {
        //先获取对应的音效资源
        AudioClip doorCloseClip = await SoundManager.Instance.LoadClipAsync(SoundManager.Instance.AudioClipKeys.MainDoorCloseKey);

        //随后将加载出来的音频加进检查字典
        m_AudioCheckDict[doorCloseClip] = false;     
 
        //根据当前玩家选择的语音改变储存的字眼（音效不变）
        string doorKeyword = LeanLocalization.GetTranslationText(DoorKeyword);
        
        //获取翻译后的字眼后将音效和字眼一起存进字典
        m_KeywordAudioDict[doorKeyword] = doorCloseClip;
    }
    #endregion


    #region 主要函数
    private async void PlayGame()   //开始游戏
    {
        //载入一楼大厅场景
        SceneManager.LoadScene("FirstFloor");

        //播放一楼BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        //停止播放下雨音效
        SoundManager.Instance.StopAudioPlay(false);
    }



    private void StartTextAnimations()
    {
        FirstPartText.gameObject.SetActive(true);       //激活第一段文本

        //先开始第一段文本的打字效果
        Coroutine firstPartTextCoroutine = StartCoroutine(TypeText(FirstPartText, FirstPartText.text, () =>
        {
            TipText.gameObject.SetActive(true);         //激活提示文本，提醒玩家按空格或点击以继续游戏

            //等待玩家按空格或点击鼠标
            Coroutine firstWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
            {
                SecondPartText.gameObject.SetActive(true);      //激活第二段文本
                TipText.gameObject.SetActive(false);            //取消激活提示文本


                //然后开始第二段文本的打字效果
                Coroutine secondPartTextCoroutine = StartCoroutine(TypeText(SecondPartText, SecondPartText.text, () =>
                {
                    //等待1秒
                    Coroutine delayCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(1f, () =>
                    {
                        LastPartText.gameObject.SetActive(true);       //激活最后一段文本


                        //然后开始最后一段文本的打字效果
                        Coroutine lastPartCoroutine = StartCoroutine(TypeText(LastPartText, LastPartText.text, () =>
                        {
                            TipText.gameObject.SetActive(true);        //激活提示文本

                            //等待玩家按空格或点击鼠标
                            Coroutine lastWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
                            {
                                //Debug.Log("Last Coroutine done!.");

                                //淡出界面
                                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                            }));

                            generatedCoroutines.Add(lastWaitForInputCoroutine);     //将协程加进列表
                        }));

                        generatedCoroutines.Add(lastPartCoroutine);                 //将协程加进列表
                    }));

                    generatedCoroutines.Add(delayCoroutine);                        //将协程加进列表
                }));

                generatedCoroutines.Add(secondPartTextCoroutine);                   //将协程加进列表
            }));

            generatedCoroutines.Add(firstWaitForInputCoroutine);                    //将协程加进列表
        }));

        generatedCoroutines.Add(firstPartTextCoroutine);                            //将协程加进列表
    }




    protected override IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, Action onTypingCompleted = null)
    {
        isTyping = true;        //表示正在打字（防止正在打字时按空格会关闭UI）


        int totalLength = fullText.Length;      //文本总长度，用于决定打字机何时结束
        int visibleCount = 0;                   //显示的文字数量


        textComponent.maxVisibleCharacters = 0;  //一开始什么都不显示

        //开始打字
        while (visibleCount < totalLength)
        {
            //检测玩家是否按空格
            if (PlayerInputHandler.Instance.IsSpacePressed)
            {
                textComponent.maxVisibleCharacters = totalLength;  //玩家按下空格后，瞬间显示所有文本

                //等待0.15秒再退出，否则如果此函数结束后的下一个函数也需要按空格时，可能会导致按一次空格响应多个函数
                yield return new WaitForSeconds(0.15f);  
                break;  //退出循环
            }


            //检查是否到了需要播放音效的字眼
            foreach (var keyword in m_KeywordAudioDict.Keys)
            {
                //检查整段文字的这一小段（从参数1到参数2，也就是目前玩家可以看到的部分）是否包含字眼
                if (fullText.Substring(0, visibleCount).Contains(keyword))
                {
                    //检查该音效是否已经播放过了，防止重复播放
                    if (!m_AudioCheckDict[m_KeywordAudioDict[keyword]] )
                    {
                        m_AudioCheckDict[m_KeywordAudioDict[keyword]] = true;

                        //播放该字眼对应的音效
                        SoundManager.Instance.PlaySFX(m_KeywordAudioDict[keyword], KeyWordAudioVolume);
                    }                   
                }
            }


            //检查是否在标签的开头
            if (fullText[visibleCount] == '<')
            {
                //跳过整个标签，直到标签的结尾（也就是>符号）。跳过的方式为不更新可以显示的文字数量，但是依然增加visibleCount变量
                while (visibleCount < totalLength && fullText[visibleCount] != '>')
                {
                    visibleCount++;
                }
            }

            visibleCount++;  //增加可以显示的文字数量
            textComponent.maxVisibleCharacters = visibleCount;  //更新可以显示的文字数量

            yield return new WaitForSeconds(typeSpeed);  //等待一段时间后再打下一个字
        }

        isTyping = false;
        onTypingCompleted?.Invoke();      //回调函数，用于某个文本全部显示完后执行一些逻辑
    }
    #endregion
}