using System.Collections.Generic;
using UnityEngine;



public class UIConst    //用于存储界面的名称
{
    public const string PlayerStatusBar = "PlayerStatusBar";    //玩家状态栏。在HealthBar脚本里初始化
    public const string TransitionStagePanel = "TransitionStagePanel";    //进入二阶段文字
}




public class UIManager
{
    private static UIManager m_Instance;
    public static UIManager Instance    //单例模式（整局游戏只存在一个此类的实例）
    {
        get
        {
            if (m_Instance == null)     //第一次检查
            {
                m_Instance = new UIManager();
            }
            return m_Instance;
        }
    }


    private Transform m_UIRoot;
    public Transform UIRoot     //所有UI的跟节点（最顶层的父物体）
    {
        get
        {
            if (m_UIRoot == null)
            {
                GameObject canvasObject = GameObject.Find("Canvas");

                if (canvasObject != null)
                {
                    m_UIRoot = canvasObject.transform;
                }
                else
                {
                    m_UIRoot = new GameObject("Canvas").transform;
                }               
            }
            return m_UIRoot;
        }
    }




    public Dictionary<string, BasePanel> PanelDict;      //存放已打开界面的字典（里面存储的都是正在打开的界面）

    Dictionary<string, string> m_PathDict;       //使用字典存储界面的配置路径表
    Dictionary<string, GameObject> m_PrefabDict;     //预制件缓存字典









    private UIManager()     //构造函数
    {
        InitDicts();
    }



    private void InitDicts()    //初始化字典
    {
        PanelDict = new Dictionary<string, BasePanel>();
        m_PrefabDict = new Dictionary<string, GameObject>();

        m_PathDict = new Dictionary<string, string>()   //初始化所有界面的路径
        {
            { UIConst.TransitionStagePanel, "Others/TransitionStagePanel"},
            { UIConst.PlayerStatusBar, "Others/PlayerStatusBar"}
        };
    }




    //打开界面
    public BasePanel OpenPanel(string name)
    {
        //检查界面是否已经打开
        BasePanel panel = null;

        if (PanelDict.TryGetValue(name, out panel))     //此函数会尝试从字典中寻找第一个参数的元素并赋值给第二个参数，最后返回true如果在字典中找到了第一个参数对应的元素
        {
            Debug.LogError("This panel is already opened: " +  name);
            return null;
        }


        //检查路径是否有配置
        string path = "";

        if (!m_PathDict.TryGetValue(name, out path))
        {
            Debug.LogError("Something wrong with the name or path of this panel: " + name);
            return null;
        }


        //使用缓存的预制件
        GameObject panelPrefab = null;

        if (!m_PrefabDict.TryGetValue(name, out panelPrefab))
        {
            string realPath = "Prefab/UI/Panels/" + path;    //如果没有被加载过，则加载出来并放入缓存字典

            panelPrefab = Resources.Load<GameObject>(realPath);     //通过Load函数从Assets中寻找资源赋值（必须在Resources文件夹下面）
            m_PrefabDict.Add(name, panelPrefab);    //加入存放界面预制件的字典
        }


        //打开界面
        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);    //生成出来，并使它成为UIRoot的一个子节点

        panel = panelObject.GetComponent<BasePanel>();
        PanelDict.Add(name, panel);       //加入存放已打开界面的字典
        return panel;
    }

    //关闭界面
    public bool ClosePanel(string name)
    {
        BasePanel panel = null;

        if (!PanelDict.TryGetValue (name, out panel))     //检查界面是否已打开，没打开的话则报错
        {
            Debug.LogError("This panel is not opened yet: " + name);
            return false;
        }


        if (panel.CanvasGroup != null)
        {
            panel.FadeOut(panel.CanvasGroup, 1f);       //如果可以淡出的话优先淡出
        }
        else
        {
            panel.ClosePanel();
        }

        return true;
    }



    public void ChangePanelLayer(BasePanel thisPanel)   //改变UI的渲染层级
    {

    }
}