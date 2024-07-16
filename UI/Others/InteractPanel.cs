using UnityEngine.UI;
using UnityEngine;
using System;



public class InteractPanel : BasePanel     //互动按键，给予玩家自己决定是否打开某些界面（比如拾取武器），而不是触发了触发器后自动打开界面
{
    public event Action OnInteractKeyPressed;     //接收方为需要玩家触发的物体（比如事件，拾取武器等）

    public static InteractPanel Instance { get; private set; }





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
    }

    private void Update() 
    {
        //持续检查玩家是否按下互动按键
        if (!IsRemoved && PlayerInputHandler.Instance.IsInteractKeyPressed)       
        {
            OnInteractKeyPressed?.Invoke();     //调用事件
        }
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
                UIManager.Instance.ImportantPanelList.Add(this);    //将该界面加进列表，以在重置游戏时不被删除
            }
        }      
    }
    #endregion


    #region 主要函数
    //需要做的：玩家靠近一些物体后打开此界面，离开物体后淡出此界面。且此界面的坐标应更改为物体坐标（随触发的物体改变）
    public override void ClosePanel()
    {
        //淡出界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }


    public void ClearAllSubscriptions()         //删除所有事件绑定的函数
    {
        OnInteractKeyPressed = null;
    }
    #endregion
}