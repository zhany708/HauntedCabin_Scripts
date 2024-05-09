using UnityEngine.UI;
using UnityEngine;
using TMPro;




public class EvilTelephonePanel : PanelWithButton
{
    //�ĸ�ѡ��
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;

    public TextMeshProUGUI EventInfo;     //�¼������ı�

    //ѡ����ı����
    TextMeshProUGUI m_OptionAText;      
    TextMeshProUGUI m_OptionBText;     
    TextMeshProUGUI m_OptionCText;      
    TextMeshProUGUI m_OptionDText;     









    protected override void Awake()
    {
        base.Awake();

        //��鰴ť����Ƿ����
        if (OptionA == null || OptionB == null || OptionC == null || OptionD == null)
        {
            Debug.LogError("Some buttons are not assigned in the EvilTelephonePanel.");
            return;
        }

        if (EventInfo == null)
        {
            Debug.LogError("EventInfo is not assigned in the EvilTelephonePanel.");
            return;
        }

        //Ĭ�ϰ�ťΪ����һ��ѡ���ť
        firstSelectedButton = OptionA.gameObject;


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


    private void Start()
    {
        panelName = "Test";


        //��ʹ�ô��ֻ�ǰ��ǰ�����ı��Ĳ�����ɫ
        string textVersionA = EventInfo.text.Replace("answer", "<color=red>answer</color>");
        string textVersionB = textVersionA.Replace("inside", "<color=blue>inside</color>");
        EventInfo.text = textVersionB;

        StartTextAnimations();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        SetButtons(false);      //����մ�ʱȡ���������а�ť
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
}