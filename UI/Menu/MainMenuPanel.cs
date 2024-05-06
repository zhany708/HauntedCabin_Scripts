using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenuPanel : PanelWithButton
{
    public Button PlayButton;
    public Button QuitButton;







    protected override void Awake()
    {
        base.Awake();

        //默认按钮为“开始游戏”按钮
        firstSelectedButton = PlayButton.gameObject;
    }


    private async void Start()
    {
        //检查按钮组件是否存在
        if (PlayButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the MainMenuPanel.");
            return;
        }

        //将按钮和函数绑定起来
        PlayButton.onClick.AddListener(() => PlayGame() );
        QuitButton.onClick.AddListener(() => QuitGame() );
   

        //初始化界面名字
        if (panelName  == null)
        {
            panelName = UIManager.Instance.UIKeys.MainMenuPanel;
        }
        

        //播放主界面BGN
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.MyVeryOwnDeadShip, true, SoundManager.Instance.MusicVolume);
    }






    private async void PlayGame()
    {
        //载入一楼大厅场景
        SceneManager.LoadScene("FirstFloor");

        //播放一楼BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);

        //从字典中移除，表示界面没打开
        UIManager.Instance.PanelDict.Remove(panelName);
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