using UnityEngine;
using System;
using TMPro;



public class InteractPanel : BasePanel     //互动按键，给予玩家自己决定是否打开某些界面（比如拾取武器），而不是触发了触发器后自动打开界面
{
    public event Action OnInteractKeyPressed;       //接收方为需要玩家触发的物体（比如事件，拾取武器等）

    public static InteractPanel Instance { get; private set; }



    RectTransform m_PanelTransform;                 //界面的坐标组件
    TextMeshProUGUI m_LetterText;                   //字母文本（玩家需要按的键）   

    Vector2 m_PositionOffset = Vector2.zero;        //距离目标坐标的偏移量

    bool m_IsActionCalled = false;                  //表示事件绑定的逻辑已经调用了（防止多次调用）







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
        if (!IsRemoved && PlayerInputHandler.Instance.IsInteractKeyPressed && !m_IsActionCalled)       
        {
            OnInteractKeyPressed?.Invoke();     //调用事件
        }
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
            //清除事件绑定的函数
            ClearAllSubscriptions();
            SetIsActionCalled(false);       //重置布尔，以便后续的调用
        }
    }


    //设置界面的坐标，需要加上偏移量
    public void SetPositionWithOffset(Vector2 thisPos)
    {
        m_PanelTransform.position = thisPos + m_PositionOffset;
    }



    public void ClearAllSubscriptions()         //删除所有事件绑定的函数
    {
        OnInteractKeyPressed = null;
    }
    #endregion


    #region 其余函数
    private void InitializeComponents()
    {
        m_LetterText = GetComponentInChildren<TextMeshProUGUI>();
        if (m_LetterText == null)
        {
            Debug.LogError("LetterText is not assigned in the " + name);
            return;
        }

        m_PanelTransform = GetComponent<RectTransform>();
        if (m_PanelTransform == null)
        {
            Debug.LogError("RectTransform is not assigned in the " + name);
            return;
        }


        //设置此界面的淡入/出时长
        FadeDuration = 0;
    }
    #endregion


    #region Setters
    public void SetIsActionCalled(bool isTrue)
    {
        m_IsActionCalled = isTrue;
    }

    //设置界面的偏移量（因为不同的物体可能因为大小不同而需要不同的偏移量）
    public void SetPositionOffset(Vector2 thisPos)
    {
        m_PositionOffset = thisPos;
    }
    #endregion
}