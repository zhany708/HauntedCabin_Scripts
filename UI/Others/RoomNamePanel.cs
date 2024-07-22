using TMPro;
using UnityEngine;



//用于玩家进入房间后显示房间名
public class RoomNamePanel : BasePanel
{
    public static RoomNamePanel Instance { get; private set; }


    //需要做的：在编辑器中确定了时长后将变量范围改成Private
    public float DisplayDuration = 1.5f;                 //显示时长



    TextMeshProUGUI m_RoomNameText;     //文本组件









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

        OnFadeInFinished += StartTextAnimations;    //彻底淡入后再开始打字
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

        OnFadeInFinished -= StartTextAnimations;
    }
    #endregion


    #region 主要函数
    public override void OpenPanel(string name)
    {
        panelName = name;

        Fade(CanvasGroup, FadeInAlpha, FadeDuration, false);     //淡入（禁止射线阻挡）
    }




    private void StartTextAnimations()
    {
        m_RoomNameText.gameObject.SetActive(true);       //激活文本组件

        //显示文本
        Coroutine textCoroutine = StartCoroutine(TypeText(m_RoomNameText, m_RoomNameText.text));

        //显示一定时间后淡出界面
        Coroutine ClosePanelCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(DisplayDuration, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);     //淡出
        }));

        generatedCoroutines.Add(textCoroutine);
        generatedCoroutines.Add(ClosePanelCoroutine);
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
        m_RoomNameText = GetComponent<TextMeshProUGUI>();       //获取界面下的文本组件
        if (m_RoomNameText == null)
        {
            Debug.LogError("RoomNameText component is not assigned in the: " + gameObject.name);
            return;
        }

        //需要做的：在编辑器中确定了时长后将变量范围改成Private，并删除此处的检查变量逻辑
        if (DisplayDuration == null)
        {
            Debug.LogError("DisplayDuration component is not assigned in the: " + gameObject.name);
            return;
        }
    }         
    #endregion
}