using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;



public class EvilTelephonePanel : PanelWithButton
{
    public static Action OnResultFinished;      //接受事件方为E_EvilTelephone脚本

    public TextMeshProUGUI EventInfo;     //事件背景文本
    public TextMeshProUGUI ResultText;    //选项结果文本
    public TextMeshProUGUI TipText;       //提示文本（提示玩家按空格或点击）


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


    PlayerStatusBar m_PlayerStatusBar;      //玩家的状态栏UI



    //用于按钮函数的枚举
    private enum ButtonAction
    {
        OptionA,
        OptionB,
        OptionC,
        OptionD
    }





    protected override void Awake()
    {
        CheckComponents();      //检查所有组件

        //默认按钮为“第四个选项”按钮
        firstSelectedButton = OptionD.gameObject;
    }


    private void Start()
    {
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

        SetButtons(false);      //界面刚打开时取消激活结果文本，提示文本和所有按钮
        ResultText.gameObject.SetActive(false);
        TipText.gameObject.SetActive(false);

        //界面完全淡出后调用此函数
        OnFadeOutFinished += ClosePanel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
    }




    public override void ClosePanel()
    {
        //延迟0.5秒后再关闭界面，并且执行事件回调
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(0.5f, () =>
        {
            base.ClosePanel();

            OnResultFinished?.Invoke();
        }));

        generatedCoroutines.Add(ClosePanelCoroutine);     //将协程加进列表
    }




    private void OnButtonClicked(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.OptionA:
                //赋值选项的结果文本
                ResultText.text = $"???: 'Snacks, my favorite snacks!'\n\nSanity <#3D88FF>+1";

                //改变玩家的属性
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Sanity, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionB:
                //赋值选项的结果文本
                ResultText.text = $"???: 'The little pies can't escape my sight...'\n\nKnowledge <#3D88FF>+1";

                //改变玩家的属性
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Knowledge, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionC:
                //赋值选项的结果文本
                ResultText.text = $"???: 'Baby, give me a kiss...'\n\nSanity <#3D88FF>-1";

                //改变玩家的属性
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Sanity, -1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionD:              
                //赋值选项的结果文本
                ResultText.text = $"???: 'Naughty kid must be punished!'\n\nStrength <#FF6B6B>-1";

                //改变玩家的属性
                m_PlayerStatusBar.ChangePropertyValue(PlayerProperty.Strength, -1f);

                CommonLogicForOptions();
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
    }




    private void CommonLogicForOptions()        //所有选项的通用逻辑
    {
        SetButtons(false);      //取消激活所有按钮
        ResultText.gameObject.SetActive(true);     //激活结果文本


        //先开始结果文本的打字效果
        Coroutine resultTextCoroutine = StartCoroutine(TypeText(ResultText, ResultText.text, () =>       
        {
            //Debug.Log("First Coroutine done!.");

            TipText.gameObject.SetActive(true);     //激活提示文本，提醒玩家按空格或点击以继续游戏

            //等待玩家按空格或点击鼠标
            Coroutine waitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>     
            {
                //Debug.Log("Second Coroutine done!.");

                //淡出界面
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
            }));

            generatedCoroutines.Add(waitForInputCoroutine);    //将协程加进列表
        }));

        generatedCoroutines.Add(resultTextCoroutine);      //将协程加进列表
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
        if (EventInfo.text.Length > 0)      //先检查介绍文本是否有字
        {
            Coroutine eventInfoCoroutine = StartCoroutine(TypeText(EventInfo, EventInfo.text, () =>
            {
                SetButtons(true);       //事件介绍完毕后，激活所有按钮

                //同时打字所有选项按钮的文本
                Coroutine OptionATextCoroutine = StartCoroutine(TypeText(m_OptionAText, m_OptionAText.text));
                Coroutine OptionBTextCoroutine = StartCoroutine(TypeText(m_OptionBText, m_OptionBText.text));
                Coroutine OptionCTextCoroutine = StartCoroutine(TypeText(m_OptionCText, m_OptionCText.text));
                Coroutine OptionDTextCoroutine = StartCoroutine(TypeText(m_OptionDText, m_OptionDText.text));

                generatedCoroutines.Add(OptionATextCoroutine);       //将协程加进列表
                generatedCoroutines.Add(OptionBTextCoroutine);
                generatedCoroutines.Add(OptionCTextCoroutine);
                generatedCoroutines.Add(OptionDTextCoroutine);
            }));

            generatedCoroutines.Add(eventInfoCoroutine);       //将协程加进列表
        }

        else
        {
            Debug.LogError("EventInfo text is empty.");
            return;
        }
    }





    private void CheckComponents()
    {
        //检查按钮组件和事件背景文本组件是否存在
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


        //获取所有按钮的文本组件
        m_OptionAText = OptionA.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionBText = OptionB.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionCText = OptionC.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionDText = OptionD.GetComponentInChildren<TextMeshProUGUI>();


        m_PlayerStatusBar = FindObjectOfType<PlayerStatusBar>();    //寻找带有此脚本的物体


        //检查按钮的文本组件是否存在
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