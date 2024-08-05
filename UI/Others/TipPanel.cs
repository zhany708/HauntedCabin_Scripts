using UnityEngine;
using System;
using TMPro;
using Lean.Localization;



//提示界面，用于在一些情况下告诉玩家需要做什么
public class TipPanel : BasePanel     
{
    public static TipPanel Instance { get; private set; }



    TextMeshProUGUI m_TipPanelText;                    //界面文本 
    
    






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
            panelName = UIManager.Instance.UIKeys.TipPanel;
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
    #endregion


    #region 主要函数
    //更新界面文本。每次打开提示界面前都需要执行的逻辑
    public void UpdatePanelText(string thisPhraseKey)
    {
        //根据当前语言赋值文本
        if (LeanLocalization.CurrentLanguages != null)
        {
            m_TipPanelText.text = LeanLocalization.GetTranslationText(thisPhraseKey);
        }
    } 
    #endregion


    #region 其余函数
    private void InitializeComponents()
    {
        m_TipPanelText = GetComponentInChildren<TextMeshProUGUI>();
        if (m_TipPanelText == null)
        {
            Debug.LogError("some components are not assigned in the " + gameObject.name);
            return;
        }



        //设置此界面的淡入/出时长
        FadeDuration = 0.5f;
    }
    #endregion
}