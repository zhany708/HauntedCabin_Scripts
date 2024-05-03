using TMPro;
using UnityEngine;
using DG.Tweening;


public class TransitionStagePanel : BasePanel
{
    TextMeshProUGUI m_TransitionStageText;      //�ı����

    float m_DisplayDuration = 10f;





    protected override void Awake()
    {
        base.Awake();

        m_TransitionStageText = GetComponentInChildren<TextMeshProUGUI>();
    }


    private void OnEnable()
    {
        //���׵�������ɾ�����棬��ֹDoTween����
        OnFadeOutFinished += base.ClosePanel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= base.ClosePanel;
    }





    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //����

        DisplayText(m_TransitionStageText);     //��ʾ�ı�
        StartCoroutine(ClosePanelAfterDelay(m_DisplayDuration));     //��ʾһ��ʱ����Զ��رս���
    }


    
    public override void ClosePanel()
    {
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //����
    } 
}