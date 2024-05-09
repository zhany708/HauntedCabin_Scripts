using UnityEngine.UI;
using UnityEngine;
using TMPro;




public class EvilTelephonePanel : PanelWithButton
{
    public TextMeshProUGUI EventInfo;     //�¼������ı�
    public TextMeshProUGUI ResultText;     //ѡ�����ı�


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




    //���ڵ����ť������ö��
    private enum ButtonAction
    {
        OptionA,
        OptionB,
        OptionC,
        OptionD
    }





    protected override void Awake()
    {
        base.Awake();

        CheckComponents();      //����������

        //Ĭ�ϰ�ťΪ�����ĸ�ѡ���ť
        firstSelectedButton = OptionD.gameObject;
    }


    private void Start()
    {
        panelName = "Test";


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

        SetButtons(false);      //����մ�ʱȡ���������а�ť
        ResultText.gameObject.SetActive(false);     //����մ�ʱȡ������ѡ��Ľ���ı�
    }





    private void OnButtonClicked(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.OptionA:
                //
                break;

            case ButtonAction.OptionB:
                SetButtons(false);      //ȡ���������а�ť

                ResultText.text = $"???: 'The little pies can't escape my sight...'\n\nKnowledge <#3D88FF>+1";
                ResultText.gameObject.SetActive(true);     //��ֵ�ı���Ϻ󼤻�ѡ��Ľ���ı�
                break;

            case ButtonAction.OptionC:
                //
                break;

            case ButtonAction.OptionD:
                //
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
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
        StartCoroutine(TypeText(EventInfo, EventInfo.text, 0.05f, () =>
        {
            SetButtons(true);       //�¼�������Ϻ󣬼������а�ť

            //ͬʱ��������ѡ�ť���ı�
            StartCoroutine(TypeText(m_OptionAText, m_OptionAText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionBText, m_OptionBText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionCText, m_OptionCText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionDText, m_OptionDText.text, 0.05f));
        }));
    }





    private void CheckComponents()
    {
        //��鰴ť������¼������ı�����Ƿ����
        if (OptionA == null || OptionB == null || OptionC == null || OptionD == null)
        {
            Debug.LogError("Some buttons are not assigned in the EvilTelephonePanel.");
            return;
        }

        if (EventInfo == null || ResultText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the EvilTelephonePanel.");
            return;
        }

        


        //��ȡ���а�ť���ı����
        m_OptionAText = OptionA.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionBText = OptionB.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionCText = OptionC.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionDText = OptionD.GetComponentInChildren<TextMeshProUGUI>();

        //��鰴ť���ı�����Ƿ����
        if (m_OptionAText == null || m_OptionBText == null || m_OptionCText == null || m_OptionDText == null)
        {
            Debug.LogError("Some option texts are not assigned on the buttons in the EvilTelephonePanel.");
            return;
        }
    }
}