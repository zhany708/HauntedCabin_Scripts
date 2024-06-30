using UnityEngine;
using UnityEngine.SceneManagement;



//用于处理加载场景相关的逻辑
public class SceneManagerScript : ManagerTemplate<SceneManagerScript>
{
    #region Unity内部函数
    private void OnEnable()
    {
        //将函数与事件（Unity内部的加载场景事件）关联
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion



    //每当加载场景时调用的函数（在新场景所有物体的Awake和OnEnable函数后执行）
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {  
        //先调用各大管理器的加载场景脚本
        EnvironmentManager.Instance.OnSceneLoaded(scene, mode);
        EventManager.Instance.OnSceneLoaded(scene, mode);
        RoomManager.Instance.OnSceneLoaded(scene, mode);
        ScreenplayManager.Instance.OnSceneLoaded(scene, mode);
        UIManager.Instance.OnSceneLoaded(scene, mode);

        //再调用其余管理器的加载场景脚本 
        PostProcessController.Instance.OnSceneLoaded(scene, mode);
        EnemyPool.Instance.OnSceneLoaded(scene, mode);

        //先调用具体的某个UI界面的加载场景脚本
        PlayerStatusBar.Instance.OnSceneLoaded(scene, mode);
    }
}