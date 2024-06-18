using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using Lean.Localization;


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

    //用于四个结果的文本（从Lean Localization那调用时需要的string）
    public string ResultA_PhraseKey;
    public string ResultB_PhraseKey;
    public string ResultC_PhraseKey;
    public string ResultD_PhraseKey;


    //按钮的文本组件
    TextMeshProUGUI m_OptionAText;      
    TextMeshProUGUI m_OptionBText;     
    TextMeshProUGUI m_OptionCText;      
    TextMeshProUGUI m_OptionDText;





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
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        //界面完全淡出后调用此函数
        OnFadeOutFinished += ClosePanel;

        OnFadeInFinished += StartTextAnimations;    //界面完全淡入后调用此函数
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
        OnFadeInFinished -= StartTextAnimations;
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
        //需要做的：等确认面板做好后，只需要保存这一行即可，下面的都可以删除（因为判断玩家选项和相关的逻辑都已经在传递到确认面板中的事件里了）
        //UIManager.Instance.OpenConfirmPanel(() => LogicPassToConfirmPanel(action));


        switch (action)
        {
            case ButtonAction.OptionA:
                //赋值选项的结果文本
                SetLocalizedText(ResultA_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Sanity, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionB:
                //赋值选项的结果文本
                SetLocalizedText(ResultB_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Knowledge, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionC:
                //赋值选项的结果文本
                SetLocalizedText(ResultC_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Sanity, -1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionD:
                //赋值选项的结果文本
                SetLocalizedText(ResultD_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Strength, -1f);

                CommonLogicForOptions();
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
    }

    private void LogicPassToConfirmPanel(ButtonAction action)           //本函数用于向确认面板传递选项相关的逻辑（玩家按下确认后才会执行下面的逻辑）
    {
        switch (action)
        {
            case ButtonAction.OptionA:
                //赋值选项的结果文本
                SetLocalizedText(ResultA_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Sanity, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionB:
                //赋值选项的结果文本
                SetLocalizedText(ResultB_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Knowledge, 1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionC:
                //赋值选项的结果文本
                SetLocalizedText(ResultC_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Sanity, -1f);

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionD:
                //赋值选项的结果文本
                SetLocalizedText(ResultD_PhraseKey);

                //改变玩家的属性
                PlayerStatusBar.Instance.ChangePropertyValue(PlayerProperty.Strength, -1f);

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



    

    private void SetLocalizedText(string phraseKey)
    {
        if (LeanLocalization.CurrentLanguages != null && ResultText != null)
        {
            ResultText.text = LeanLocalization.GetTranslationText(phraseKey);   //根据当前语言赋值文本
        }
    }


    private void SetButtons(bool isActive)      //激活或隐藏按钮
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
            EventInfo.gameObject.SetActive(true);       //激活事件介绍文本

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
            Debug.LogError("Some buttons are not assigned in the " + name);
            return;
        }

        if (EventInfo == null || ResultText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the " + name);
            return;
        }

        if (ResultA_PhraseKey == "" || ResultB_PhraseKey == "" || ResultC_PhraseKey == "" || ResultD_PhraseKey == "")
        {
            Debug.LogError("Some Lean Localization phrase keys are not written in the " + name);
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