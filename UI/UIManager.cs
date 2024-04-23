using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;



public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }



    public Dictionary<string, BasePanel> PanelDict = new Dictionary<string, BasePanel>();      //存放已打开界面的字典（里面存储的都是正在打开的界面）
    Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();     //预制件缓存字典


    Transform m_UIRoot;     //用于储存所有的UI（为了美观）









    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

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
        GameObject panelPrefab = await LoadPanelAsync(name);
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

        await LoadPanelAsync(name);
    }

    //异步加载
    private async Task<GameObject> LoadPanelAsync(string name)
    {
        //检查字典里是否有界面，如果有的话直接返回
        if (!m_PrefabDict.TryGetValue(name, out GameObject panelPrefab))
        {
            //异步加载界面
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            await handle.Task;

            //检查异步加载是否成功
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                panelPrefab = handle.Result;

                //将界面预制件存进字典
                m_PrefabDict[name] = panelPrefab;
            }

            else
            {
                Debug.LogError($"Failed to load Panel prefab: {name}");
                return null;
            }
        }

        return panelPrefab;
    }





    public void ChangePanelLayer(BasePanel thisPanel)   //改变UI的渲染层级
    {

    }


    //在Addressables里释放UI，只有这样才能释放内存
    public void ReleaseUI(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
            key = key.Substring(0, key.Length - 7);
        }


        if (m_PrefabDict.TryGetValue(key, out GameObject panelPrefab))
        {
            Addressables.Release(panelPrefab);

            //从预制件缓存字典中移除UI物体
            m_PrefabDict.Remove(key);

            Debug.Log("UI released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This UI is not loaded yet, cannot release: " + key);
        }
    }
}