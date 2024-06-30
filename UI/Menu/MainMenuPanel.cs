using UnityEngine;
using UnityEngine.UI;



public class MainMenuPanel : PanelWithButton
{
    public Button PlayButton;       //开始游戏按钮
    public Button SettingButton;    //设置界面按钮
    public Button QuitButton;       //关闭游戏按钮







    protected override void Awake()
    {
        base.Awake();

        //检查按钮组件是否存在
        if (PlayButton == null || SettingButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the MainMenuPanel.");
            return;
        }

        //默认按钮为“开始游戏”按钮
        firstSelectedButton = PlayButton.gameObject;
    }


    private async void Start()
    {
        //将按钮和函数绑定起来
        PlayButton.onClick.AddListener(() => PlayGame());
        SettingButton.onClick.AddListener(() => OpenSettingPanel());
        QuitButton.onClick.AddListener(() => QuitGame());

        
        //初始化界面名字
        if (panelName  == null)
        {
            panelName = UIManager.Instance.UIKeys.MainMenuPanel;
        }
        

        //播放主界面BGN
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.MyVeryOwnDeadShip, true);

        //当玩家第一次进游戏时
        if (EnvironmentManager.Instance.IsFirstTimeEnterGame)
        {
            //提前初始化游戏背景介绍界面
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.GameBackgroundPanel);
        }
    }






    private async void PlayGame()
    {
        //当玩家第一次进游戏时
        if (EnvironmentManager.Instance.IsFirstTimeEnterGame)
        {
            //打开游戏背景介绍界面
            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameBackgroundPanel);
        }

        //当玩家进入过一楼场景后
        else
        {
            //载入一楼大厅场景
            SceneManager.LoadScene("FirstFloor");

            //播放一楼BGM
            await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);
        }
        

        //淡出当前界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }

    private async void OpenSettingPanel()
    {
        //打开游戏设置界面
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.SettingPanel);
    }

    private void QuitGame()
    {
        //退出游戏（用于本地游戏包）
        Application.Quit();

        //在Unity编辑器内退出游玩模式
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}