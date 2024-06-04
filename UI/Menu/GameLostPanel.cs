using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameLostPanel : PanelWithButton
{
    public Button RestartButton;
    public Button QuitButton;





    protected override void Awake()
    {
        //检查按钮组件是否存在
        if (RestartButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the GameOverPanel.");
            return;
        }

        //默认按钮为“重新开始”按钮
        firstSelectedButton = RestartButton.gameObject;
    }

    private void Start()
    {
        //将按钮和函数绑定起来
        RestartButton.onClick.AddListener(() => OnRestartButtonClick());
        QuitButton.onClick.AddListener(() => OnQuitButtonClick());       
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += HandleFadeOutFinished;        //彻底淡出时执行函数
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= HandleFadeOutFinished;
    }
    






    private void OnRestartButtonClick()
    {
        //关闭界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }
   
    private void OnQuitButtonClick()
    {
        //退出游戏（用于本地游戏包）
        Application.Quit();

        //在Unity编辑器内退出游玩模式
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }



    private void HandleFadeOutFinished()
    {
        //返回主菜单
        SceneManager.LoadScene("MainMenu");

        //重置游戏的各种系统
        ResetGameSystems();
    }
}