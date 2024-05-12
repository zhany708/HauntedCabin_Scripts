using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;



public class EvilTelephonePanel : PanelWithButton
{
    public static Action OnResultFinished;      //�����¼���ΪE_EvilTelephone�ű�

    public TextMeshProUGUI EventInfo;     //�¼������ı�
    public TextMeshProUGUI ResultText;    //ѡ�����ı�
    public TextMeshProUGUI TipText;       //��ʾ�ı�����ʾ��Ұ��ո������


    //�ĸ���ť
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;



    //��ť���ı����
    TextMeshProUGUI m_OptionAText;      
    TextMeshProUGUI m_OptionBText;     
    TextMeshProUGUI m_OptionCText;      
    TextMeshProUGUI m_OptionDText;


    PlayerStatusBar m_PlayerStatusBar;      //��ҵ�״̬��UI



    //���ڰ�ť������ö��
    private enum ButtonAction
    {
        OptionA,
        OptionB,
        OptionC,
        OptionD
    }





    protected override void Awake()
    {
        CheckComponents();      //����������

        //Ĭ�ϰ�ťΪ�����ĸ�ѡ���ť
        firstSelectedButton = OptionD.gameObject;
    }


    private void Start()
    {
        //����ť�ͺ���������
        OptionA.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionA));
        OptionB.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionB));
        OptionC.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionC));
        OptionD.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionD));


        //��ʹ�ô��ֻ�ǰ��ǰ�����ı��Ĳ�����ɫ����Ҫ���Ķ������ʱ�����ں����ٴε���.Replace�������ٴ����µ�string������
        string preparedText = EventInfo.text.Replace("answer", "<color=red>answer</color>");
        EventInfo.text = preparedText;

        StartTextAnimations();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        SetButtons(false);      //����մ�ʱȡ���������ı�����ʾ�ı������а�ť
        ResultText.gameObject.SetActive(false);
        TipText.gameObject.SetActive(false);

        //������ȫ��������ô˺���
        OnFadeOutFinished += ClosePanel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
    }




    public override void ClosePanel()
    {
        //�ӳ�0.5����ٹرս��棬����ִ���¼��ص�
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(0.5f, () =>
        {
            base.ClosePanel();

            OnResultFinished?.Invoke();
        }));

        generatedCoroutines.Add(ClosePanelCoroutine);     //��Э�̼ӽ��б�
    }




    private void OnButtonClicked(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.OptionA:
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'Snacks, my favorite snacks!'\n\nSanity <#3D88FF>+1";

                //�ı���ҵ�����
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Sanity, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionB:
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'The little pies can't escape my sight...'\n\nKnowledge <#3D88FF>+1";

                //�ı���ҵ�����
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Knowledge, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionC:
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'Baby, give me a kiss...'\n\nSanity <#3D88FF>-1";

                //�ı���ҵ�����
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Sanity, -1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionD:              
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'Naughty kid must be punished!'\n\nStrength <#FF6B6B>-1";

                //�ı���ҵ�����
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Strength, -1f);

                CommonLogicForOptions();
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
    }




    private void CommonLogicForOptions()        //����ѡ���ͨ���߼�
    {
        SetButtons(false);      //ȡ���������а�ť
        ResultText.gameObject.SetActive(true);     //�������ı�


        //�ȿ�ʼ����ı��Ĵ���Ч��
        Coroutine resultTextCoroutine = StartCoroutine(TypeText(ResultText, ResultText.text, () =>       
        {
            //Debug.Log("First Coroutine done!.");

            TipText.gameObject.SetActive(true);     //������ʾ�ı���������Ұ��ո�����Լ�����Ϸ

            //�ȴ���Ұ��ո�������
            Coroutine waitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>     
            {
                //Debug.Log("Second Coroutine done!.");

                //��������
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
            }));

            generatedCoroutines.Add(waitForInputCoroutine);    //��Э�̼ӽ��б�
        }));

        generatedCoroutines.Add(resultTextCoroutine);      //��Э�̼ӽ��б�
    }



    




    private void SetButtons(bool isActive)
    {
        OptionA.gameObject.SetActive(isActive);
        OptionB.gameObject.SetActive(isActive);
        OptionC.gameObject.SetActive(isActive);
        OptionD.gameObject.SetActive(isActive);       
    }


    private void StartTextAnimations()
    {       
        if (EventInfo.text.Length > 0)      //�ȼ������ı��Ƿ�����
        {
            Coroutine eventInfoCoroutine = StartCoroutine(TypeText(EventInfo, EventInfo.text, () =>
            {
                SetButtons(true);       //�¼�������Ϻ󣬼������а�ť

                //ͬʱ��������ѡ�ť���ı�
                Coroutine OptionATextCoroutine = StartCoroutine(TypeText(m_OptionAText, m_OptionAText.text));
                Coroutine OptionBTextCoroutine = StartCoroutine(TypeText(m_OptionBText, m_OptionBText.text));
                Coroutine OptionCTextCoroutine = StartCoroutine(TypeText(m_OptionCText, m_OptionCText.text));
                Coroutine OptionDTextCoroutine = StartCoroutine(TypeText(m_OptionDText, m_OptionDText.text));

                generatedCoroutines.Add(OptionATextCoroutine);       //��Э�̼ӽ��б�
                generatedCoroutines.Add(OptionBTextCoroutine);
                generatedCoroutines.Add(OptionCTextCoroutine);
                generatedCoroutines.Add(OptionDTextCoroutine);
            }));

            generatedCoroutines.Add(eventInfoCoroutine);       //��Э�̼ӽ��б�
        }

        else
        {
            Debug.LogError("EventInfo text is empty.");
            return;
        }
    }





    private void CheckComponents()
    {
        //��鰴ť������¼������ı�����Ƿ����
        if (OptionA == null || OptionB == null || OptionC == null || OptionD == null)
        {
            Debug.LogError("Some buttons are not assigned in the EvilTelephonePanel.");
            return;
        }

        if (EventInfo == null || ResultText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the EvilTelephonePanel.");
            return;
        }


        //��ȡ���а�ť���ı����
        m_OptionAText = OptionA.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionBText = OptionB.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionCText = OptionC.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionDText = OptionD.GetComponentInChildren<TextMeshProUGUI>();


        m_PlayerStatusBar = FindObjectOfType<PlayerStatusBar>();    //Ѱ�Ҵ��д˽ű�������


        //��鰴ť���ı�����Ƿ����
        if (m_OptionAText == null || m_OptionBText == null || m_OptionCText == null || m_OptionDText == null)
        {
            Debug.LogError("Some option texts are not assigned on the buttons in the EvilTelephonePanel.");
            return;
        }

        if (m_PlayerStatusBar == null)
        {
            Debug.LogError("Cannot find any GameObject with component PlayerStatusBar.");
            return;
        }
    }
}