using UnityEngine;
using UnityEngine.SceneManagement;



//用于处理加载场景相关的逻辑
public class SceneManagerScript : ManagerTemplate<SceneManagerScript>
{
    private void OnEnable()
    {
        //将函数与事件（Unity内部的加载场景事件）关联
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    //每当加载场景时调用的函数
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerStatusBar.Instance.OnSceneLoaded(scene, mode);
    }
}