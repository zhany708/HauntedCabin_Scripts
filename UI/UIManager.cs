using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public class UIManager : ManagerTemplate<UIManager>
{
    public Dictionary<string, BasePanel> PanelDict = new Dictionary<string, BasePanel>();      //存放已打开界面的字典（里面存储的都是正在打开的界面）


    Transform m_UIRoot;     //用于储存所有的UI（为了美观）









    protected override void Awake()
    {
        base.Awake();

        //寻找画布物体，没有的话就创建一个
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
        }

        m_UIRoot = canvasObject.transform;
    }






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
        

        //异步加载后生成物体并获取物体身上的组件
        GameObject panelObject = GameObject.Instantiate(panelPrefab, m_UIRoot, false);
        BasePanel panel = panelObject.GetComponent<BasePanel>();
        if (panel == null)
        {
            Debug.LogError("No BasePanel component found on prefab: " + name);
            return;
        }

        PanelDict.Add(name, panel);
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




    public void ChangePanelLayer(BasePanel thisPanel)   //改变UI的渲染层级
    {

    }
}