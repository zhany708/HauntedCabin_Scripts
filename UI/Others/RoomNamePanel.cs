using TMPro;
using UnityEngine;
using Lean.Localization;



//用于玩家进入房间后显示房间名
public class RoomNamePanel : BasePanel
{
    public static RoomNamePanel Instance { get; private set; }



    TextMeshProUGUI m_RoomNameText;     //文本组件

    float m_DisplayDuration = 1f;                 //界面自动显示时长







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
        //此界面无需调用ClosePanel，因为会经常用到

        OnFadeInFinished += HandleFadeInFinished;    //彻底淡入后再开始打字
    }

    private void Start() 
    {
        //赋值界面名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.RoomNamePanel;
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

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeInFinished -= HandleFadeInFinished;
    }
    #endregion


    #region 主要函数
    public override void OpenPanel()
    {
        Fade(CanvasGroup, FadeInAlpha, 0f, false);     //立刻淡入，且禁止射线阻挡
    }

    public override void OpenPanel(string name)
    {
        panelName = name;

        Fade(CanvasGroup, FadeInAlpha, 0f, false);     //立刻淡入，且禁止射线阻挡
    }



    public void SetLocalizedText(string phraseKey)
    {
        if (LeanLocalization.CurrentLanguages != null && m_RoomNameText != null)
        {
            m_RoomNameText.text = LeanLocalization.GetTranslationText(phraseKey);   //根据当前语言赋值文本
        }
    }
    #endregion


    #region 其他函数
    private void InitializeComponents()     //初始化组件
    {
        m_RoomNameText = GetComponentInChildren<TextMeshProUGUI>();       //获取界面下子物体中的文本组件
        if (m_RoomNameText == null)
        {
            Debug.LogError("RoomNameText component is not assigned in the: " + gameObject.name);
            return;
        }


        FadeDuration = 0.5f;        //设置界面的淡出时长（淡入时长为0）
    }


    public void HandleFadeInFinished()
    {
        CanvasGroup.alpha = FadeInAlpha;        //重置界面的透明度

        //显示一定时间后淡出界面
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(m_DisplayDuration, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //淡出
        }));

        generatedCoroutines.Add(ClosePanelCoroutine);
    }
    #endregion
}