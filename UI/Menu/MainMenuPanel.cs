using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenuPanel : PanelWithButton
{
    public Button PlayButton;
    public Button QuitButton;








    protected async override void Start()
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


        //默认按钮为“开始游戏”按钮，随后将其设置到EventSystem
        lastSelectedButton = PlayButton.gameObject;


        base.Start();

        //播放主界面BGN
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.MyVeryOwnDeadShip, true, SoundManager.Instance.MusicVolume);
    }







    private async void PlayGame()
    {
        //载入一楼大厅场景
        SceneManager.LoadScene("FirstFloor");

        //播放一楼BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);

        ClosePanel();
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