using TMPro;
using UnityEngine;




public class HellsCallBackground : BasePanel
{
    public TextMeshProUGUI TitleText;           //剧本标题文本
    public TextMeshProUGUI FirstPartText;       //第一段文本
    public TextMeshProUGUI SecondPartText;      //第二段文本  
    public TextMeshProUGUI TipText;             //提示文本


    public float AudioVolume = 1.5f;     //界面火焰灼烧音效的音量大小





    #region Unity内部函数
    protected override void Awake()
    {
        if (TitleText == null || FirstPartText == null || SecondPartText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the " + name);
            return;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += ClosePanel;                //界面完全淡出后调用的函数
        OnFadeOutFinished += HandleFadeOutFinished;

        OnFadeInFinished += StartTextAnimations;        //界面完全淡入后调用此函数
    }

    private async void Start()
    {
        if (!UIManager.Instance.NoMoveAndAttackList.Contains(this))
        {
            UIManager.Instance.NoMoveAndAttackList.Add(this);       //界面淡入后禁止玩家移动和攻击
        }


        SoundManager.Instance.StopAudioPlay(true);      //停止当前BGM的播放

        //循环播放火焰灼烧音效
        SoundManager.Instance.PlaySFXAsync(SoundManager.Instance.AudioClipKeys.FireBurningKey, AudioVolume, true);

        //提前加载剧本BGM
        await SoundManager.Instance.LoadClipAsync(SoundManager.Instance.AudioClipKeys.FireEscape);
    }

    private void Update()
    {
        //SetBothMoveableAndAttackable(false);            //界面打开时禁止玩家移动和攻击
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= HandleFadeOutFinished;
        OnFadeInFinished -= StartTextAnimations;
    }

    private void OnDestroy()
    {
        //释放本界面所有使用过的音效     
        SoundManager.Instance.ReleaseAudioClip(SoundManager.Instance.AudioClipKeys.FireBurningKey);
    }
    #endregion


    #region 主要函数   
    public override void ClosePanel()
    {
        base.ClosePanel();

        SoundManager.Instance.StopAudioPlay(false);     //停止播放火焰灼烧音效
    }
    



    private void StartTextAnimations()
    {
        FirstPartText.gameObject.SetActive(true);       //激活第一段文本

        //先开始第一段文本的打字效果
        Coroutine firstPartTextCoroutine = StartCoroutine(TypeText(FirstPartText, FirstPartText.text, () =>
        {
            //等待0.5秒
            Coroutine delayCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(0.5f, () =>
            {
                SecondPartText.gameObject.SetActive(true);      //激活第二段文本           

                //然后开始第二段文本的打字效果
                Coroutine secondPartTextCoroutine = StartCoroutine(TypeText(SecondPartText, SecondPartText.text, () =>
                {
                    TipText.gameObject.SetActive(true);     //激活提示文本

                    //等待玩家按空格或点击鼠标
                    Coroutine waitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
                    {
                        //淡出界面
                        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                    }));

                    generatedCoroutines.Add(waitForInputCoroutine);   //将协程加进列表
                }));

                generatedCoroutines.Add(secondPartTextCoroutine);   //将协程加进列表
            }));

            generatedCoroutines.Add(delayCoroutine);        //将协程加进列表
        }));

        generatedCoroutines.Add(firstPartTextCoroutine);      //将协程加进列表
    }



    //界面隐藏时进行的逻辑
    protected async override void HandleFadeOutFinished()
    {
        base.HandleFadeOutFinished();

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.TaskPanel);        //打开任务界面

        HellsCall.Instance.StartHealthDrain();          //开始持续掉血，并且播放火焰滤镜

        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.FireEscape, true);       //播放剧本BGM
    }
    #endregion
}