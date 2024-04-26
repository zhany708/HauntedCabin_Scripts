using TMPro;



public class TransitionStagePanel : BasePanel
{
    TextMeshProUGUI m_TransitionStageText;      //文本组件

    float m_DisplayDuration = 10f;





    protected override void Awake()
    {
        base.Awake();

        m_TransitionStageText = GetComponentInChildren<TextMeshProUGUI>();
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

        //淡出后删除物体
        base.ClosePanel();
    }
}