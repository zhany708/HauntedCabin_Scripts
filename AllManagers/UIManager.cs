using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;



public class UIManager : ManagerTemplate<UIManager>
{
    //[SerializeField]    //强迫编辑器显示一个内部变量
    public SO_UIKeys UIKeys;

    //存放已打开界面的字典（里面存储的都是正在打开的界面）
    public Dictionary<string, BasePanel> PanelDict { get; private set; } = new Dictionary<string, BasePanel>();    

    //用于储存所有不可删除的UI（比如玩家状态等）
    public List<BasePanel> ImportantPanel { get; private set; } = new List<BasePanel>();   


    Transform m_UIRoot;     //用于储存所有的UI（为了美观）








    #region Unity内部函数循环
    protected override void Awake()
    {
        base.Awake();

        //寻找画布跟物体，没有的话就创建一个
        SetupRootGameObject(ref m_UIRoot, "Canvas");

        //加载新场景时不删除Canvas总组件
        DontDestroyOnLoad(m_UIRoot);
    }


    private async void Start()
    {
        //检查是否处于场景0（主菜单）
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            //游戏开始时加载开始界面
            //await OpenPanel(UIKeys.MainMenuPanel);
        }

        else
        {
            //不在主菜单时，则播放一楼BGM
            await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true);
        }
    }
    #endregion


    #region 界面相关
    //打开界面
    public async Task OpenPanel(string name)
    {
        //如果界面已经打开，则报错
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            return;
        }

        //异步加载，随后检查是否加载成功
        GameObject panelPrefab = await LoadPrefabAsync(name);
        if (panelPrefab == null)
        {
            Debug.LogError("Failed to load panel prefab: " + name);
            return;
        }


        //如果因为场景加载等原因导致画布跟物体被删除过，就重新获取
        if (m_UIRoot == null)
        {
            //寻找画布跟物体，没有的话就创建一个
            SetupRootGameObject(ref m_UIRoot, "Canvas");
        }

        //异步加载后生成物体并获取物体身上的组件
        GameObject panelObject = GameObject.Instantiate(panelPrefab, m_UIRoot, false);
        BasePanel panel = panelObject.GetComponent<BasePanel>();
        if (panel != null)
        {
            //获取组件后，打开界面
            panel.OpenPanel(name);
        }

        else
        {
            Debug.LogError("No BasePanel component found on prefab: " + name);
            return;
        }


        if (!PanelDict.ContainsKey(name))
        {
            //将界面加进储存正在打开界面的字典
            PanelDict.Add(name, panel);
        }         
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

        panel.ClosePanel();
       
        return true;
    }



    //提前加载界面（提前将预制件放入字典，防止卡顿）(跟打开界面函数几乎一模一样，只是少了生成并打开界面的步骤)
    public async Task InitPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            return;
        }

        await LoadPrefabAsync(name);
    }




    public void ChangePanelLayer(BasePanel thisPanel)   //改变界面的渲染层级
    {

    }
    #endregion


    #region 打开特定面板
    public async void OpenConfirmPanel(Action onYesAction)      //专门用于打开确认界面
    {
        ConfirmPanel.Instance.ClearAllSubscriptions();                   //先清空所有事件绑定的之前的函数
        ConfirmPanel.Instance.OnYesButtonPressed += onYesAction;         //将参数中的函数绑定到事件
        await OpenPanel(UIKeys.ConfirmPanel);                   //打开确认界面
    }

    public async void OpenInteractPanel(Action onYesAction)     //专门用于打开互动界面
    {
        InteractPanel.Instance.ClearAllSubscriptions();                  //先清空所有事件绑定的之前的函数
        InteractPanel.Instance.OnInteractKeyPressed += onYesAction;      //将参数中的函数绑定到事件
        await OpenPanel(UIKeys.InteractPanel);                  //打开互动界面
    }
    #endregion


    public void ResetGame()
    {
        foreach (Transform child in m_UIRoot)    //在场景中删除所有Canvas下的UI（通过调用ClosePanel进行彻底的删除）
        {
            BasePanel childScript = child.GetComponent<BasePanel>();
            if (childScript == null)
            {
                Debug.LogError("Cannot get the BasePanel script from the: " + child.name);
                return;
            }

            if (!ImportantPanel.Contains(childScript))      //只关闭不重要的界面
            {
                childScript.ClosePanel();    
            }           
        }
    }
}