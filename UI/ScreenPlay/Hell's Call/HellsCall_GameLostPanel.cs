using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;



public class HellsCall_GameLostPanel : BasePanel
{
    public TextMeshProUGUI FirstPartText;       //第一段文本
    public TextMeshProUGUI SecondPartText;      //第二段文本  
    public TextMeshProUGUI TipText;             //提示文本
    






    protected override void Awake()
    {
        //检查文本组件是否存在
        if (FirstPartText == null || SecondPartText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the " + name);
            return;
        }
    }

    private void Update()
    {
        SetBothMoveableAndAttackable(false);        //界面打开时禁止玩家移动和攻击
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += Restart;        //界面淡入后执行函数以重置系统

        OnFadeInFinished += StartTextAnimations;    //界面完全淡入后调用此函数
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= Restart;
        OnFadeInFinished -= StartTextAnimations;    //界面完全淡入后调用此函数
    }
    



    private void Restart()
    {
        //获取玩家组件
        Player player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Player component not found.");
            return;          
        }


        //重置玩家的各个状态和数值（血量，属性）      
        PlayerStats playerStats = player.GetComponentInChildren<PlayerStats>();     //获取玩家血条的脚本组件
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found under Player object.");
            return;
        }

        playerStats.SetCurrentHealth(playerStats.MaxHealth);    //重置玩家的血量
        PlayerStatusBar.ResetGame();                            //重置玩家的属性



        //重置游戏的各个系统（房间，事件等）
        ResetGameSystems();

        //将玩家传送回入口大堂（必须在重置游戏后，否则顺序错误会导致无法正常生成新的房间）
        player.gameObject.transform.position = Vector2.zero;    

        SetBothMoveableAndAttackable(true);    //使玩家可以移动和攻击
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
}