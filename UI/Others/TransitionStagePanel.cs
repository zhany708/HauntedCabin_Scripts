using TMPro;
using UnityEngine;



public class TransitionStagePanel : BasePanel
{
    public TextMeshProUGUI TransitionStageText;      //文本组件

    float m_DisplayDuration = 5f;





    protected override void Awake()
    {
        base.Awake();

        if (TransitionStageText == null )
        {
            Debug.LogError("TransitionStageText component is not assigned in the TransitionStagePanel.");
            return;
        }
    }


    private void OnEnable()
    {       
        OnFadeOutFinished += ClosePanel;            //彻底淡出后再删除界面，并且打开任务界面
        OnFadeOutFinished += OpenTaskPanel;

        OnFadeInFinished += StartTextAnimations;    //彻底淡入后再开始打字
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= OpenTaskPanel;

        OnFadeInFinished -= StartTextAnimations;
    }





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

    private async void OpenTaskPanel()        //打开任务界面
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.TaskPanel);
    }
}