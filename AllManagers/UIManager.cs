using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;




public class UIManager : ManagerTemplate<UIManager>
{
    //[SerializeField]    //强迫编辑器显示一个内部变量
    public SO_UIKeys UIKeys;

    //存放已打开界面的字典（里面存储的都是正在打开的界面）
    public Dictionary<string, BasePanel> PanelDict { get; private set; } = new Dictionary<string, BasePanel>();

    //用于储存所有重置游戏时不可删除的UI（比如玩家状态，小地图等）
    public List<BasePanel> ImportantPanelList { get; private set; } = new List<BasePanel>();
    //用于切换场景时不显示出来的重要UI界面（该列表里的界面一定也储存在ImportantPanelList列表里）
    public List<BasePanel> DontDisplayPanelList { get; private set; } = new List<BasePanel>();


    Transform m_UIRoot;     //用于储存所有的UI（为了美观）








    #region Unity内部函数循环
    protected override void Awake()
    {
        base.Awake();

        //寻找画布跟物体，没有的话就创建一个
        SetupRootGameObject(ref m_UIRoot, "Canvas");
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
    public void ClosePanel(string name, bool isFadeOut)
    {
        if (!PanelDict.TryGetValue (name, out BasePanel panel))     //检查界面是否已打开，没打开的话则报错
        {
            Debug.LogError("This panel is not opened yet: " + name);
            return;
        }


        //淡出
        if (isFadeOut)
        {
            panel.Fade(panel.CanvasGroup, BasePanel.FadeOutAlpha, panel.FadeDuration, false);
        }
        //关闭
        else
        {
            panel.ClosePanel();
        }


        if (PanelDict.ContainsKey(name))
        {
            //将界面从字典移除
            PanelDict.Remove(name);
        }
    }



    //提前加载界面（跟打开界面函数几乎一模一样，只是少了生成并打开界面的步骤)
    public async Task InitPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            return;
        }

        //提前将预制件放入ManagerTemplate的字典，防止卡顿（不是PanelDict字典，是储存预制件的字典）
        await LoadPrefabAsync(name);
    }




    public void ChangePanelLayer(BasePanel thisPanel)   //改变界面的渲染层级
    {

    }
    #endregion


    #region 打开特定界面（确认界面，互动界面等）
    public async void OpenConfirmPanel(Action onYesAction, BasePanel connectedPanel)              //专门用于打开确认界面
    {
        connectedPanel.SetInteractableAndBlocksRaycasts(false);         //禁止连接界面的互动性，防止错误的选择位于底部的界面的按钮


        if (ConfirmPanel.Instance == null)
        {
            await OpenPanel(UIKeys.ConfirmPanel);                       //异步加载并打开确认界面
        }
        else
        {
            ConfirmPanel.Instance.OpenPanel();                          //如果之前加载过了，则直接打开界面
        }

        
        ConfirmPanel.Instance.ClearAllSubscriptions();                  //先清空所有事件绑定的之前的函数
        ConfirmPanel.Instance.OnYesButtonPressed += onYesAction;        //将参数中的函数绑定到事件
        ConfirmPanel.Instance.SetConnectedPanel(connectedPanel);        //将连接的面板赋值给确认界面       
    }



    //打开互动界面（参数中的事件不能为Task类型）
    public async void OpenInteractPanel(Action onYesAction)     
    {
        if (InteractPanel.Instance == null)
        {
            await OpenPanel(UIKeys.InteractPanel);                              //异步加载并打开互动界面
        }
        else
        {
            InteractPanel.Instance.OpenPanel();                                 //如果之前加载过了，则直接打开界面
        }


        InteractPanel.Instance.ClearAllSubscriptions();                         //先清空所有事件绑定的之前的函数
        InteractPanel.Instance.OnInteractKeyPressed += onYesAction;             //将参数中的函数绑定到事件       
        InteractPanel.Instance.SetPositionWithOffset();                         //设置界面的坐标
    }


    //打开房间名界面
    public async void OpenRoomNamePanel(string roomNamePhraseKey)     
    {
        if (RoomNamePanel.Instance == null)
        {
            await OpenPanel(UIKeys.RoomNamePanel);                              //异步加载并打开房间名界面
        }
        else
        {
            //当界面处于激活状态时（即正在显示其它房间名）
            if (!RoomNamePanel.Instance.IsRemoved)
            {
                RoomNamePanel.Instance.ClearAllCoroutinesAndTweens();           //先删除上次打开界面时的协程，防止界面很快就淡出
                RoomNamePanel.Instance.SetLocalizedText(roomNamePhraseKey);     //根据当前语言显示新的房间名
                RoomNamePanel.Instance.HandleFadeInFinished();                  //开始新的协程，以便一段时间后界面再次淡出
                return;
            }

            RoomNamePanel.Instance.OpenPanel();                                 //如果之前加载过了，则直接打开界面
        }


        RoomNamePanel.Instance.SetLocalizedText(roomNamePhraseKey);             //将参数传递给界面，以根据当前语言显示房间名
    }
    #endregion


    #region 其余函数
    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            //只有非第一次进入主菜单才会重置游戏
            if (!EnvironmentManager.Instance.IsFirstTimeEnterGame)
            {
                //重置游戏
                ResetGame();
            }
        }

        //进入一楼场景
        else if (scene.name == SceneManagerScript.FirstFloorSceneName)
        {
            DisplayImportantPanelsWithConditions();      //激活所有不带按钮的重要界面
        }

        else
        {
            Debug.Log("We only have two scenes now, please check the parameters!");
        }
    }


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


            //普通的界面
            if (!ImportantPanelList.Contains(childScript))      
            {
                //Debug.Log("This panel will be deleted: " + childScript.name);

                //关闭不重要的界面
                childScript.ClosePanel();
            }

            //重要的界面
            else
            {
                //淡出重要的界面
                childScript.Fade(childScript.CanvasGroup, BasePanel.FadeOutAlpha, childScript.FadeDuration, false);
            }
        }
    }

    //显示一些符合条件的重要界面
    public void DisplayImportantPanelsWithConditions()
    {
        //更快速的淡入重要界面的方式，但会导致测试时直接进入一楼场景后无法正常实施功能
        foreach (BasePanel childScript in ImportantPanelList)
        {
            //检查界面是否有按钮，或者界面是否处于禁止显示列表中
            if (!(childScript is PanelWithButton) && !DontDisplayPanelList.Contains(childScript) )
            {
                //如果都满足则淡入界面
                childScript.Fade(childScript.CanvasGroup, childScript.FadeInAlpha, childScript.FadeDuration, true);
            }         
        }
    }
    #endregion
}