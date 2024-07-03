using TMPro;
using UnityEngine;




public class HellsCallBackground : BasePanel
{
    public TextMeshProUGUI TitleText;           //剧本标题文本
    public TextMeshProUGUI FirstPartText;       //第一段文本
    public TextMeshProUGUI SecondPartText;      //第二段文本  
    public TextMeshProUGUI TipText;             //提示文本








    #region Unity内部函数
    protected override void Awake()
    {
        if (TitleText == null || FirstPartText == null || SecondPartText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the GameBackgroundPanel.");
            return;
        }
    }

    private void Update()
    {
        SetBothMoveableAndAttackable(false);        //界面打开时禁止玩家移动和攻击
    }


    private void OnEnable()
    {
        
        OnFadeOutFinished += ClosePanel;        //界面完全淡出后调用的函数
        OnFadeOutFinished += StartHealthDrain;

        OnFadeInFinished += StartTextAnimations;    //界面完全淡入后调用此函数
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= StartHealthDrain;
        OnFadeInFinished -= StartTextAnimations;
    }
    #endregion

    
    #region 主要函数
    public async override void ClosePanel()
    {
        base.ClosePanel();

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.TaskPanel);        //打开任务界面

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



    private void StartHealthDrain()     //开始持续掉血，并且播放火焰滤镜
    {
        //HellsCall.Instance.SetDoFireEffect(true);       //设置布尔，从而开始火焰滤镜
        HellsCall.Instance.StartHealthDrain();       
    }
    #endregion
}