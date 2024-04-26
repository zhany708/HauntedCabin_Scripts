using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PauseMenuPanel : PanelWithButton       //挂载在Canvas游戏物体上，因为整个游戏过程中都有可能会用到此界面
{
    public Button ResumeButton;
    public Button MainMenuButton;
    public Button QuitButton;

    public GameObject PauseMenuUI;

    bool isGamePaused = false;







    protected override void Awake()
    {
        base.Awake();

        if (PauseMenuUI == null)
        {
            Debug.LogError("PauseMenuPanel is not assigned in the Canvas.");
            return;
        }

        //检查按钮组件是否存在
        if (ResumeButton == null || MainMenuButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the PauseMenuPanel.");
            return;
        }


        //将按钮和函数绑定起来
        ResumeButton.onClick.AddListener(() => Resume());
        MainMenuButton.onClick.AddListener(() => BackToMainMenu());
        QuitButton.onClick.AddListener(() => QuitGame());


        if (lastSelectedButton == null)
        {
            //默认按钮为“开始游戏”按钮，随后将其设置到EventSystem
            lastSelectedButton = ResumeButton.gameObject;
        }         
    }


    protected override void Start()
    {
        base.Start();

        panelName = UIManager.Instance.UIKeys.PauseMenuPanel;

        //将UI界面加进字典
        if (!UIManager.Instance.PanelDict.ContainsKey(panelName) )
        {
            UIManager.Instance.PanelDict.Add(panelName, this);
        }      
    }


    protected override void Update()
    {
        base.Update();

        //这个脚本持续检查玩家是否按下Esc，在暂停过程中按下的话则恢复游戏，否则就暂停游戏    需要做的：添加一个计时器，防止玩家反复按Esc时界面频繁的打开和关闭
        if (playerInputHandler.IsEscPressed)
        {
            //游戏暂停时
            if (isGamePaused)
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






    //恢复游戏
    private void Resume()
    {
        PauseMenuUI.SetActive(false);

        //恢复游戏的时间流逝
        Time.timeScale = 1f;
      
        isGamePaused = false;
    }

    //暂停游戏
    private void Pause()
    {
        PauseMenuUI.SetActive(true);

        //暂停游戏的时间流逝
        Time.timeScale = 0f;

        isGamePaused = true;
    }





    private void BackToMainMenu()
    {
        //先恢复游戏，随后返回主菜单
        Resume();

        //返回主界面场景
        SceneManager.LoadScene("MainMenu");

        //ClosePanel();
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