using UnityEngine.UI;
using UnityEngine;
using System;



public class ConfirmPanel : PanelWithButton     //用于询问玩家是否确认自己的选择（每当调用这个界面前，需要先将逻辑绑定到事件）
{
    public event Action OnYesButtonPressed;     //接收方为需要做出选择的所有UI界面（比如事件中的选项，拾取武器等）

    public static ConfirmPanel Instance { get; private set; }

    public Button ConfirmButton;
    public Button NoButton;


    BasePanel m_ConnectedPanel;                 //连接界面（需要打开此界面的那个面板）




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


        //检查按钮组件是否存在
        if (ConfirmButton == null || NoButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the " + name);
            return;
        }

        //默认按钮为“确认”按钮
        firstSelectedButton = ConfirmButton.gameObject;

        //设置此界面的淡入/出时长
        FadeDuration = 0;
    }

    private void Start()
    {
        //将按钮和函数绑定起来
        ConfirmButton.onClick.AddListener(() => OnYesButtonClick());
        NoButton.onClick.AddListener(() => OnNoButtonClick());


        //赋值界面名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.ConfirmPanel;
        }


        //检查该界面是否是唯一保留的那个
        if (Instance == this)
        {
            //将该界面加进列表，以在重置游戏时不被删除
            if (!UIManager.Instance.ImportantPanelList.Contains(this))
            {
                UIManager.Instance.ImportantPanelList.Add(this);    
            }
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
            //界面打开后加进列表，从而禁止玩家移动和攻击
            if (!openedPanelsWithButton.Contains(this))
            {
                openedPanelsWithButton.Add(this);
            }
        }
        //淡出
        else
        {
            //界面关闭后移出列表，从而恢复玩家的移动和攻击
            if (openedPanelsWithButton.Contains(this))
            {
                openedPanelsWithButton.Remove(this);
            }
        }

        SetTopPriorityButton();
    }
    #endregion


    #region 按钮相关
    private void OnYesButtonClick()
    {
        OnYesButtonPressed?.Invoke();      //回调事件，将逻辑绑定到这个事件，从而进行不同的逻辑

        //淡出界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }

    private void OnNoButtonClick()
    {
        //淡出界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);

        //恢复连接界面的互动性
        m_ConnectedPanel.SetInteractableAndBlocksRaycasts(true);
    }    



    public void ClearAllSubscriptions()         //删除所有事件绑定的函数
    {
        OnYesButtonPressed = null;
    }
    #endregion


    #region Setters
    public void SetConnectedPanel(BasePanel thisPanel)
    {
        m_ConnectedPanel = thisPanel;
    }
    #endregion
}