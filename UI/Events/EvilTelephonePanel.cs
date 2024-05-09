using UnityEngine.UI;
using UnityEngine;
using TMPro;




public class EvilTelephonePanel : PanelWithButton
{
    //四个选项
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;

    public TextMeshProUGUI EventInfo;     //事件背景文本

    //选项的文本组件
    TextMeshProUGUI m_OptionAText;      
    TextMeshProUGUI m_OptionBText;     
    TextMeshProUGUI m_OptionCText;      
    TextMeshProUGUI m_OptionDText;     









    protected override void Awake()
    {
        base.Awake();

        //检查按钮组件是否存在
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

        //默认按钮为“第一个选项”按钮
        firstSelectedButton = OptionA.gameObject;


        //获取所有按钮的文本组件
        m_OptionAText = OptionA.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionBText = OptionB.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionCText = OptionC.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionDText = OptionD.GetComponentInChildren<TextMeshProUGUI>();

        //检查按钮的文本组件是否存在
        if (m_OptionAText == null || m_OptionBText == null || m_OptionCText == null || m_OptionDText == null)
        {
            Debug.LogError("Some option texts are not assigned on the buttons in the EvilTelephonePanel.");
            return;
        }     
    }


    private void Start()
    {
        panelName = "Test";


        //在使用打字机前提前更改文本的部分颜色
        string textVersionA = EventInfo.text.Replace("answer", "<color=red>answer</color>");
        string textVersionB = textVersionA.Replace("inside", "<color=blue>inside</color>");
        EventInfo.text = textVersionB;

        StartTextAnimations();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        SetButtons(false);      //界面刚打开时取消激活所有按钮
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
            SetButtons(true);       //事件介绍完毕后，激活所有按钮

            //同时打字所有选项按钮的文本
            StartCoroutine(TypeText(m_OptionAText, m_OptionAText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionBText, m_OptionBText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionCText, m_OptionCText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionDText, m_OptionDText.text, 0.05f));
        }));
    }
}