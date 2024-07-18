using UnityEngine;
using System;
using TMPro;



public class InteractPanel : BasePanel     //互动按键，给予玩家自己决定是否打开某些界面（比如拾取武器），而不是触发了触发器后自动打开界面
{
    public event Action OnInteractKeyPressed;       //接收方为需要玩家触发的物体（比如事件，拾取武器等）

    public static InteractPanel Instance { get; private set; }


    public Vector2 OffsetPos = Vector2.zero;       //用于打开互动界面时的偏移量

    public Player Player                            //Lazy load（只在需要变量时才尝试获取组件，而不是一次性的放在某个Unity内部函数中）
    {
        get
        {
            if (m_Player == null)
            {
                m_Player = FindAnyObjectByType<Player>();
                //如果尝试获取组件后Player变量仍然为空的话，则报错
                if (m_Player == null)
                {
                    Debug.Log("Cannot get the reference of the Player component in the " + name);
                }
            }
            return m_Player;
        }
        private set { }
    }
    private Player m_Player;


    //表示是否允许打开界面（比如玩家因为某些原因需要在OnTriggerStay2D中再次打开此界面），所有的允许玩家反悔的物体或界面都需要跟此布尔进行交互
    public bool IsOpenable { get; private set; } = true;            
    public bool IsActionCalled { get; private set; } = false;              //表示事件绑定的逻辑已经调用了（防止多次调用）



    RectTransform m_PanelTransform;                 //界面的坐标组件
    TextMeshProUGUI m_LetterText;                   //字母文本（玩家需要按的键）   


    
    






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

    private void OnEnable()
    {
        IsOpenable = true;              //开启界面后设置布尔
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


    //设置界面的坐标，需要加上偏移量（界面统一显示在角色右侧）
    public void SetPositionWithOffset()
    {
        m_PanelTransform.position = (Vector2)Player.transform.position + OffsetPos;
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

        
        if (OffsetPos == Vector2.zero)
        {
            Debug.LogError("OffsetPosForInteractPanel is not assigned in the " + name);
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
    #endregion
}