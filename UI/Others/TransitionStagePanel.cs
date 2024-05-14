using TMPro;
using UnityEngine;



public class TransitionStagePanel : BasePanel
{
    public TextMeshProUGUI TransitionStageText;      //�ı����

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
        OnFadeOutFinished += ClosePanel;            //���׵�������ɾ�����棬���Ҵ��������
        OnFadeOutFinished += OpenTaskPanel;

        OnFadeInFinished += StartTextAnimations;    //���׵�����ٿ�ʼ����
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

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //���루û�������赲��       
    }



    private void StartTextAnimations()
    {
        TransitionStageText.gameObject.SetActive(true);       //�����ı����

        //��ʾ�ı�
        Coroutine textCoroutine = StartCoroutine(TypeText(TransitionStageText, TransitionStageText.text));

        //��ʾһ��ʱ��󵭳�����
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(m_DisplayDuration, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //����
        }));

        generatedCoroutines.Add(textCoroutine);
        generatedCoroutines.Add(ClosePanelCoroutine);
    }

    private async void OpenTaskPanel()        //���������
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.TaskPanel);
    }
}