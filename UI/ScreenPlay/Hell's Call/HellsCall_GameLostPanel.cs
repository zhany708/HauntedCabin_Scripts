using TMPro;
using UnityEngine;



public class HellsCall_GameLostPanel : BasePanel
{
    public TextMeshProUGUI TitleText;           //标题文本
    public TextMeshProUGUI FirstPartText;       //第一段文本
    public TextMeshProUGUI SecondPartText;      //第二段文本
    public TextMeshProUGUI TipText;             //提示文本
    





    #region Unity内部函数
    protected override void Awake()
    {
        //检查文本组件是否存在
        if (TitleText == null || FirstPartText == null || SecondPartText == null || TipText == null)
        {
            Debug.LogError("Some Text components are not assigned in the " + name);
            return;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += Restart;               //界面淡出后执行函数以重置系统

        OnFadeInFinished += StartTextAnimations;    //界面完全淡入后调用此函数
    }

    private async void Start()
    {
        if (!UIManager.Instance.NoMoveAndAttackList.Contains(this))
        {
            UIManager.Instance.NoMoveAndAttackList.Add(this);       //界面淡入后禁止玩家移动和攻击
        }


        //提前加载一楼BGM
        await SoundManager.Instance.LoadClipAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment);

        //停止播放剧本BGM
        SoundManager.Instance.StopAudioPlay(true);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= Restart;
        OnFadeInFinished -= StartTextAnimations;    //界面完全淡入后调用此函数
    }
    #endregion


    #region 主要函数
    private async void Restart()
    {
        //重置游戏的各个系统（房间，事件等）
        ResetGameSystems();


        //获取玩家组件
        Player player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found.");
            return;          
        }

        
        //重置玩家的各个状态和数值（血量，属性）      
        Stats playerStats = player.GetComponentInChildren<Stats>();     //获取玩家血条的脚本组件
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found under Player object.");
            return;
        }

        playerStats.SetCurrentHealth(playerStats.MaxHealth);             //重置玩家的血量


        //将玩家传送回入口大堂（必须在重置游戏后，否则顺序错误会导致无法正常生成新的房间）
        player.gameObject.transform.position = new Vector2(0, -3.5f); 


        //重新播放一楼BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true);
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
    #endregion
}