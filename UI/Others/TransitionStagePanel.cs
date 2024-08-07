using TMPro;
using UnityEngine;



//目前游戏中用不到
public class TransitionStagePanel : BasePanel
{
    public TextMeshProUGUI TransitionStageText;      //文本组件

    float m_DisplayDuration = 5f;




    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();

        if (TransitionStageText == null )
        {
            Debug.LogError("TransitionStageText component is not assigned in the TransitionStagePanel.");
            return;
        }
    }

    protected override void OnEnable()
    {       
        OnFadeOutFinished += ClosePanel;            //彻底淡出后再删除界面，并且打开任务界面
        OnFadeOutFinished += OpenScreenPlayBackgroundPanel;

        OnFadeInFinished += StartTextAnimations;    //彻底淡入后再开始打字
    }

    private async void Start()
    {
        await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.HellsCallBackground);      //提前加载剧本背景界面
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= OpenScreenPlayBackgroundPanel;

        OnFadeInFinished -= StartTextAnimations;
    }
    #endregion


    #region 主要函数
    public override void OpenPanel(string name)
    {
        panelName = name;

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //淡入（没有射线阻挡）       
    }




    private void StartTextAnimations()
    {
        TransitionStageText.gameObject.SetActive(true);       //激活文本组件

        //显示文本
        Coroutine textCoroutine = StartCoroutine(TypeText(TransitionStageText, TransitionStageText.text));

        //显示一定时间后淡出界面
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(m_DisplayDuration, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //淡出
        }));

        generatedCoroutines.Add(textCoroutine);
        generatedCoroutines.Add(ClosePanelCoroutine);
    }



    private async void OpenScreenPlayBackgroundPanel()        
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallBackground);   //打开剧本背景界面
    }
    #endregion
}