using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




public class MainMenuPanel : PanelWithButton
{
    public Button PlayButton;                   //开始游戏按钮
    public Button SettingButton;                //设置界面按钮
    public Button QuitButton;                   //关闭游戏按钮



    public float MainPanelBgmVolume = 1.5f;     //主界面BGM的音量大小






    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();

        //检查按钮组件是否存在
        if (PlayButton == null || SettingButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the " + name);
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
           

        //提前初始化游戏设置界面
        await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.SettingPanel);

        //当玩家第一次进游戏时
        if (EnvironmentManager.Instance.IsFirstTimeEnterGame)
        {
            //提前初始化游戏背景介绍界面
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.GameBackgroundPanel);
        }


        //播放主界面BGN
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.MyVeryOwnDeadShip, true, MainPanelBgmVolume);
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += HandleFadeOutFinished;     //主菜单界面完全关闭后再打开其余界面
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= HandleFadeOutFinished;

        //当UIManager的预制件字典里有设置界面时（即玩家没有打开并关闭过设置界面）
        if (UIManager.Instance.PrefabDict.ContainsKey(UIManager.Instance.UIKeys.SettingPanel) )
        {
            UIManager.Instance.ReleasePrefab(UIManager.Instance.UIKeys.SettingPanel);
        }
    }
    #endregion


    #region 按钮绑定的函数
    private void PlayGame()
    {
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
    #endregion


    #region 主要函数
    private async void HandleFadeOutFinished()
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
            await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true);
        }
    }
    #endregion
}