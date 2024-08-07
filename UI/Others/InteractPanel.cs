using UnityEngine;
using System;
using TMPro;
using Lean.Localization;



public class InteractPanel : BasePanel     //互动按键，给予玩家自己决定是否打开某些界面（比如拾取武器），而不是触发了触发器后自动打开界面
{
    public event Action OnInteractKeyPressed;       //接收方为需要玩家触发的物体（比如事件物体，拾取武器等）

    public static InteractPanel Instance { get; private set; }


    public string InteractTextPhraseKey;            //本界面的翻译文本




    //表示是否允许打开界面（比如玩家因为某些原因需要在OnTriggerStay2D中再次打开此界面），所有的允许玩家反悔的物体或界面都需要跟此布尔进行交互
    public bool IsOpenable { get; private set; } = true;            
    public bool IsActionCalled { get; private set; } = false;              //表示事件绑定的逻辑已经调用了（防止多次调用）



    const string m_InteractKey = "F";               //进行互动的按键（默认F）

    //中文：“按{0}{1}”，英文：“Press {0} to {1}”。{0}为需要按下的按键（可更改），{1}为按下按键后进行的具体功能（如拾取匕首，开始仪式等）
    TextMeshProUGUI m_PanelText;                    //界面文本 

    string m_InteractText;                          //互动相关的文本（如拾取霰弹枪等）
    
    






    #region Unity内部函数
    protected override void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }


        InitializeComponents();         //初始化组件
    }

    protected override void OnEnable()
    {
        IsOpenable = true;              //开启界面后设置布尔

        OnFadeOutFinished += ResetInteractText;         //界面淡出后重置互动文本
    }

    private void Start()
    {
        //赋值界面名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.InteractPanel;
        }


        //检查该界面是否是唯一保留的那个
        if (Instance == this)
        {
            if (!UIManager.Instance.ImportantPanelList.Contains(this))
            {
                UIManager.Instance.ImportantPanelList.Add(this);        //将该界面加进列表，以在重置游戏时不被删除
            }

            if (!UIManager.Instance.DontDisplayPanelList.Contains(this))
            {
                UIManager.Instance.DontDisplayPanelList.Add(this);      //将该界面加进列表，以在进入一楼界面时不显示出来
            }
        }      
    }

    private void Update() 
    {
        //持续检查玩家是否按下互动按键（需要确保界面打开，且事件还没有执行）
        if (!IsRemoved && PlayerInputHandler.Instance.IsInteractKeyPressed && !IsActionCalled)       
        {
            OnInteractKeyPressed?.Invoke();     //调用事件

            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);       //淡出界面

            IsOpenable = false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= ResetInteractText;
    }
    #endregion


    #region 主要函数
    public override void Fade(CanvasGroup targetGroup, float targetAlpha, float duration, bool blocksRaycasts)
    {
        base.Fade(targetGroup, targetAlpha, duration, blocksRaycasts);


        //在淡入的情况下
        if (targetAlpha != FadeOutAlpha)
        {

        }
        //淡出
        else
        {           
            ClearAllSubscriptions();        //清除事件绑定的函数
            SetIsActionCalled(false);       //重置布尔，以便后续的调用
        }
    }


    //更新界面文本。每次打开互动界面前都需要执行的逻辑
    public void UpdatePanelText()
    {
        string tempText = m_PanelText.text;         //创建临时string，以用于下面的转换

        m_PanelText.text = string.Format(tempText, m_InteractKey, m_InteractText);
    } 



    public void ClearAllSubscriptions()         //删除所有事件绑定的函数
    {
        OnInteractKeyPressed = null;
    }

    private void ResetInteractText()         //重置界面文本，以用于下次打开
    {
        if (LeanLocalization.CurrentLanguages != null)
        {
            m_PanelText.text = LeanLocalization.GetTranslationText(InteractTextPhraseKey);
        }
    }
    #endregion


    #region 其余函数
    private void InitializeComponents()
    {
        if (InteractTextPhraseKey == null)
        {
            Debug.LogError("One or more components are missing on " + gameObject.name);
            return;
        }


        m_PanelText = GetComponentInChildren<TextMeshProUGUI>();
        if (m_PanelText == null)
        {
            Debug.LogError("LetterText is not assigned in the " + name);
            return;
        }



        //设置此界面的淡入/出时长
        FadeDuration = 0;
    }
    #endregion


    #region Setters
    public void SetIsOpenable(bool isTrue)
    {
        IsOpenable = isTrue;
    }

    public void SetIsActionCalled(bool isTrue)
    {
        IsActionCalled = isTrue;
    }

    public void SetInteractText(string thisPhraseKey)
    {
        if (LeanLocalization.CurrentLanguages != null)
        {
            m_InteractText = LeanLocalization.GetTranslationText(thisPhraseKey);
        }      
    }
    #endregion
}