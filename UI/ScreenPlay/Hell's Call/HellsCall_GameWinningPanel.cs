using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;



public class HellsCall_GameWinningPanel : PanelWithButton
{
    public TextMeshProUGUI WinningText;       //胜利文本

    public Button RestartButton;
    public Button QuitButton;





    protected override void Awake()
    {
        //检查所有组件是否存在
        if (WinningText == null || RestartButton == null || QuitButton == null)
        {
            Debug.LogError("Some UI components are not assigned in the " + name);
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



    private void SetButtons(bool isActive)      //激活或隐藏按钮
    {
        RestartButton.gameObject.SetActive(isActive);
        QuitButton.gameObject.SetActive(isActive);     
    }


    private void StartTextAnimations()
    {       
        if (WinningText.text.Length > 0)      //先检查胜利文本是否有字
        {
            WinningText.gameObject.SetActive(true);       //激活胜利文本

            Coroutine winningTextCoroutine = StartCoroutine(TypeText(WinningText, EventInfo.text, () =>
            {
                SetButtons(true);       //事件介绍完毕后，激活所有按钮
            }));

            generatedCoroutines.Add(winningTextCoroutine);       //将协程加进列表
        }

        else
        {
            Debug.LogError("WinningText is empty.");
            return;
        }
    }
}