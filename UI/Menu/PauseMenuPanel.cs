using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PauseMenuPanel : PanelWithButton       //整个游戏过程中都会用到此界面
{
    public static PauseMenuPanel Instance { get; private set; }

    

    public Button ResumeButton;
    public Button MainMenuButton;
    public Button QuitButton;



    bool m_IsGamePaused = false;       //表示游戏是否处于暂停状态





    protected override void Awake()
    {
        base.Awake();

        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }



        //检查按钮组件是否存在
        if (ResumeButton == null || MainMenuButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the PauseMenuPanel.");
            return;
        }

        //默认按钮为“恢复”按钮
        firstSelectedButton = ResumeButton.gameObject;


        //设置此界面的淡入值
        FadeInAlpha = 0.75f;
        FadeDuration = 0;
    }


    private void Start()
    {
        //将该界面加进列表，以在重置游戏时不被删除（不能放在Awake或OnEnable中，以防顺序错误）
        //UIManager.Instance.ImportantPanel.Add(this);

        //将按钮和函数绑定起来
        ResumeButton.onClick.AddListener(() => Resume());
        MainMenuButton.onClick.AddListener(() => BackToMainMenu());
        QuitButton.onClick.AddListener(() => QuitGame());


        //设置当前界面的名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.PauseMenuPanel;
        }           
    }


    protected override void Update()
    {
        //当界面打开时才调用父类的Update函数
        if (CanvasGroup.alpha == FadeInAlpha)
        {
            base.Update();
        }


        //持续检查玩家是否按下Esc，在暂停过程中按下的话则恢复游戏，否则就暂停游戏
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //游戏暂停时
            if (m_IsGamePaused)
            {
                Resume();
            }

            //游戏正常时
            else
            {
                Pause();
            }
        }                  
    }

    //重写函数，因为此界面游戏开始时就存在
    protected override void OnEnable() 
    {
        OnFadeInFinished += base.OnEnable;
        OnFadeOutFinished += base.OnDisable;
    }

    protected override void OnDisable()
    {
        OnFadeInFinished -= base.OnEnable;
        OnFadeOutFinished -= base.OnDisable;
    }







    //恢复游戏
    private void Resume()
    {
        //关闭界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);

        //恢复游戏的时间流逝
        Time.timeScale = 1f;
        
        m_IsGamePaused = false;
        SetBothMoveableAndAttackable(true);     //允许玩家移动和攻击
    }


    //暂停游戏
    private void Pause()
    {
        //打开界面
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);

        //暂停游戏的时间流逝
        Time.timeScale = 0f;        

        m_IsGamePaused = true;
        SetBothMoveableAndAttackable(false);    //不允许玩家移动和攻击
    }


    //返回主菜单
    private void BackToMainMenu()
    {
        //先恢复游戏，随后返回主菜单
        Resume();

        //返回主界面场景
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            //游戏开始时加载开始界面
            Debug.Log("You are already in the Main Menu!");
        }

        else
        {
            //不在主菜单时，则返回主菜单
            SceneManager.LoadScene("MainMenu");

            //重置游戏的各种系统
            //ResetGameSystems();
        }
    }


    //退出游戏
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