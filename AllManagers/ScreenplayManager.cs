using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;




public class ScreenplayManager : ManagerTemplate<ScreenplayManager>     //用于管理剧本相关的逻辑的
{
    public SO_ScreenplayKeys ScreenplayKeys;


    Transform m_ScreenplayRoot;     //用于储存所有的剧本（为了美观）



    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();

        //寻找画布跟物体，没有的话就创建一个
        SetupRootGameObject(ref m_ScreenplayRoot, "ScreenplayRoot");
    }
    #endregion


    #region 剧本相关
    public async Task OpenScreenplay(string name)       //打开剧本
    {
        //异步加载，随后检查是否加载成功
        GameObject screenPlayPrefab = await LoadPrefabAsync(name);
        if (screenPlayPrefab == null)
        {
            Debug.LogError("Failed to load screenplay prefab: " + name);
            return;
        }


        //异步加载后生成物体并获取物体身上的组件（物体放在m_ScreenplayRoot下面）
        GameObject screenplayObject = GameObject.Instantiate(screenPlayPrefab, m_ScreenplayRoot, false);

        if (screenplayObject == null)
        {
            Debug.LogError("ScreenplayObject instantiated fail: " + name);
            return;
        }
    }
    #endregion


    #region 其余函数
    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            //重置游戏
            ResetGame();
        }
    }


    public void ResetGame()     //重置游戏，删除所有剧本物体
    {
        foreach (Transform child in m_ScreenplayRoot)    //在场景中删除所有剧本物体
        {
            BaseScreenplay screenplay = child.GetComponent<BaseScreenplay>();
            if (screenplay == null)
            {
                Debug.LogError("Cannot get the BaseScreenplay component in the : " + child.name);
                return;
            }

            //先调用剧本脚本的重置游戏函数，随后再删除剧本物体
            screenplay.ResetGame();

            Destroy(child.gameObject);
        }
    }
    #endregion
}