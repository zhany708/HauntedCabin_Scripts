using TMPro;
using UnityEngine;



public class TransitionStagePanel : BasePanel
{
    TextMeshProUGUI m_TransitionStageText;      //文本组件

    float m_DisplayDuration = 5f;





    protected override void Awake()
    {
        base.Awake();

        m_TransitionStageText = GetComponentInChildren<TextMeshProUGUI>();
    }


    private void OnEnable()
    {
        //彻底淡出后再删除界面
        OnFadeOutFinished += ClosePanel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= ClosePanel;
    }





    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //淡入

        //显示文本
        Coroutine textCoroutine = StartCoroutine(TypeText(m_TransitionStageText, m_TransitionStageText.text) );

        //显示一定时间后淡出界面
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(m_DisplayDuration, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //淡出
        }));

        generatedCoroutines.Add(textCoroutine);
        generatedCoroutines.Add(ClosePanelCoroutine);
    }
}