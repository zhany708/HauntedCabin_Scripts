using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;




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
    Dictionary<string, GameObject> m_PrefabDict;     //预制件缓存字典








    //构造函数
    private UIManager()     
    {
        InitDicts();
    }


    //初始化字典
    private void InitDicts()    
    {
        PanelDict = new Dictionary<string, BasePanel>();
        m_PrefabDict = new Dictionary<string, GameObject>();
    }






    //打开界面
    public IEnumerator OpenPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            yield break;
        }

        GameObject panelPrefab;
        if (!m_PrefabDict.TryGetValue(name, out panelPrefab))
        {
            //异步加载预制件
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            yield return new WaitUntil(() => handle.IsDone);

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load the panel prefab: " + name);
                yield break;
            }

            panelPrefab = handle.Result;
            m_PrefabDict[name] = panelPrefab;  //缓存加载的预制件
        }


        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        BasePanel panel = panelObject.GetComponent<BasePanel>();
        PanelDict.Add(name, panel);
        yield return panel;  //返回UI，用于进一步的处理(可选)
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



    //提前加载界面（提前将预制件放入字典，防止卡顿）(跟打开界面函数一模一样，只是少了生成并打开界面的步骤)
    public IEnumerator InitPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            yield break;
        }

        GameObject panelPrefab;
        if (!m_PrefabDict.TryGetValue(name, out panelPrefab))
        {
            //异步加载预制件
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            yield return new WaitUntil(() => handle.IsDone);

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load the panel prefab: " + name);
                yield break;
            }

            panelPrefab = handle.Result;
            m_PrefabDict[name] = panelPrefab;  //缓存加载的预制件
        }
    }



    public void ChangePanelLayer(BasePanel thisPanel)   //改变UI的渲染层级
    {

    }
}