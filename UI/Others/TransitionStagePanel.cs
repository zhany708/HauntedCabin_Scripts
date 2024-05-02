using TMPro;
using UnityEngine;
using DG.Tweening;


public class TransitionStagePanel : BasePanel
{
    TextMeshProUGUI m_TransitionStageText;      //文本组件

    float m_DisplayDuration = 10f;





    protected override void Awake()
    {
        base.Awake();

        m_TransitionStageText = GetComponentInChildren<TextMeshProUGUI>();
    }


    private void OnEnable()
    {
        //彻底淡出后再删除界面，防止DoTween报错
        OnFadeOutFinished += base.ClosePanel;
    }

    private void OnDisable()
    {
        OnFadeOutFinished -= base.ClosePanel;
    }





    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //淡入

        DisplayText(m_TransitionStageText);     //显示文本
        StartCoroutine(ClosePanelAfterDelay(m_DisplayDuration));     //显示一定时间后自动关闭界面
    }


    
    public override void ClosePanel()
    {
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //淡出
    } 
}