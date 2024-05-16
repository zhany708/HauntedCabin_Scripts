using TMPro;
using UnityEngine;




public class HellsCallPanel : BasePanel
{
    public TextMeshProUGUI TitleText;           //�籾�����ı�
    public TextMeshProUGUI FirstPartText;       //��һ���ı�
    public TextMeshProUGUI SecondPartText;      //�ڶ����ı�  
    public TextMeshProUGUI TipText;             //��ʾ�ı�





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
        SetBothMoveableAndAttackable(false);        //�����ʱ��ֹ����ƶ��͹���
    }


    private void OnEnable()
    {
        
        OnFadeOutFinished += ClosePanel;        //������ȫ��������ô˺���

        OnFadeInFinished += StartTextAnimations;    //������ȫ�������ô˺���
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
        OnFadeInFinished -= StartTextAnimations;
    }


    
    public async override void ClosePanel()
    {
        base.ClosePanel();

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.TaskPanel);        //���������

        SetBothMoveableAndAttackable(true);    //����Ϸ�ָ�����
    }
    



    private void StartTextAnimations()
    {
        FirstPartText.gameObject.SetActive(true);       //�����һ���ı�

        //�ȿ�ʼ��һ���ı��Ĵ���Ч��
        Coroutine firstPartTextCoroutine = StartCoroutine(TypeText(FirstPartText, FirstPartText.text, () =>
        {
            //�ȴ�0.5��
            Coroutine delayCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(0.5f, () =>
            {
                SecondPartText.gameObject.SetActive(true);      //����ڶ����ı�           

                //Ȼ��ʼ�ڶ����ı��Ĵ���Ч��
                Coroutine secondPartTextCoroutine = StartCoroutine(TypeText(SecondPartText, SecondPartText.text, () =>
                {
                    TipText.gameObject.SetActive(true);     //������ʾ�ı�

                    //�ȴ���Ұ��ո�������
                    Coroutine waitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
                    {
                        //��������
                        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                    }));

                    generatedCoroutines.Add(waitForInputCoroutine);   //��Э�̼ӽ��б�
                }));

                generatedCoroutines.Add(secondPartTextCoroutine);   //��Э�̼ӽ��б�
            }));

            generatedCoroutines.Add(delayCoroutine);        //��Э�̼ӽ��б�
        }));

        generatedCoroutines.Add(firstPartTextCoroutine);      //��Э�̼ӽ��б�
    }
}