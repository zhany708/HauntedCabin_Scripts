using UnityEngine.UI;
using UnityEngine;
using TMPro;




public class EvilTelephonePanel : PanelWithButton
{
    public TextMeshProUGUI EventInfo;     //事件背景文本
    public TextMeshProUGUI ResultText;     //选项结果文本


    //四个按钮
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;




    //按钮的文本组件
    TextMeshProUGUI m_OptionAText;      
    TextMeshProUGUI m_OptionBText;     
    TextMeshProUGUI m_OptionCText;      
    TextMeshProUGUI m_OptionDText;




    //用于点击按钮函数的枚举
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

        CheckComponents();      //检查所有组件

        //默认按钮为“第四个选项”按钮
        firstSelectedButton = OptionD.gameObject;
    }


    private void Start()
    {
        panelName = "Test";


        //将按钮和函数绑定起来
        OptionA.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionA));
        OptionB.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionB));
        OptionC.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionC));
        OptionD.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionD));


        //在使用打字机前提前更改文本的部分颜色（需要更改多个单词时可以在后面再次调用.Replace，无须再创建新的string变量）
        string preparedText = EventInfo.text.Replace("answer", "<color=red>answer</color>");
        EventInfo.text = preparedText;

        StartTextAnimations();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        SetButtons(false);      //界面刚打开时取消激活所有按钮
        ResultText.gameObject.SetActive(false);     //界面刚打开时取消激活选项的结果文本
    }





    private void OnButtonClicked(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.OptionA:
                //
                break;

            case ButtonAction.OptionB:
                SetButtons(false);      //取消激活所有按钮

                ResultText.text = $"???: 'The little pies can't escape my sight...'\n\nKnowledge <#3D88FF>+1";
                ResultText.gameObject.SetActive(true);     //赋值文本完毕后激活选项的结果文本
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
            SetButtons(true);       //事件介绍完毕后，激活所有按钮

            //同时打字所有选项按钮的文本
            StartCoroutine(TypeText(m_OptionAText, m_OptionAText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionBText, m_OptionBText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionCText, m_OptionCText.text, 0.05f));
            StartCoroutine(TypeText(m_OptionDText, m_OptionDText.text, 0.05f));
        }));
    }





    private void CheckComponents()
    {
        //检查按钮组件和事件背景文本组件是否存在
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
}