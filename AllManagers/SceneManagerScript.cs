using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;



//用于处理加载场景相关的逻辑
public class SceneManagerScript : ManagerTemplate<SceneManagerScript>
{
    //游戏的所有场景名
    public const string MainMenuSceneName = "MainMenu";
    public const string FirstFloorSceneName = "FirstFloor";






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

    private void OnDestroy()
    {
        //DOTween.KillAll();      //杀死所有DOTween的逻辑，防止报错（不能放在这，否则加载一楼场景时小地图等UI不会正常淡入）
    }
    #endregion


    #region 主要函数
    //每当加载场景时调用的函数（在新场景所有物体的Awake和OnEnable函数后，Start函数前执行）
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //先调用各大管理器的加载场景脚本（这里的顺序很重要，因为某些管理器可能依赖另一个管理器中的布尔）    
        EventManager.Instance.OnSceneLoaded(scene, mode);
        RoomManager.Instance.OnSceneLoaded(scene, mode);
        ScreenplayManager.Instance.OnSceneLoaded(scene, mode);
        UIManager.Instance.OnSceneLoaded(scene, mode);
        EnvironmentManager.Instance.OnSceneLoaded(scene, mode);     //此管理器的执行顺序尽量放在最后（确保在RoomManager后面）

        //再调用其余管理器的加载场景脚本 
        PostProcessManager.Instance.OnSceneLoaded(scene, mode);
        EnemyPool.Instance.OnSceneLoaded(scene, mode);
        ParticlePool.Instance.OnSceneLoaded(scene, mode);

        //先调用具体的某个UI界面的加载场景脚本
        PlayerStatusBar.Instance.OnSceneLoaded(scene, mode);
    }
    #endregion
}