using TMPro;
using UnityEngine;



public class TransitionStagePanel : BasePanel
{
    TextMeshProUGUI m_TransitionStageText;      //�ı����

    float m_DisplayDuration = 5f;





    protected override void Awake()
    {
        base.Awake();

        m_TransitionStageText = GetComponentInChildren<TextMeshProUGUI>();
    }


    private void OnEnable()
    {
        //���׵�������ɾ������
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

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //����

        //��ʾ�ı�
        Coroutine textCoroutine = StartCoroutine(TypeText(m_TransitionStageText, m_TransitionStageText.text) );

        //��ʾһ��ʱ��󵭳�����
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(m_DisplayDuration, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //����
        }));

        generatedCoroutines.Add(textCoroutine);
        generatedCoroutines.Add(ClosePanelCoroutine);
    }
}