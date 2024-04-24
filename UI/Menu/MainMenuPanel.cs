using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MainMenuPanel : BasePanel
{
    public Button PlayButton;
    public Button QuitButton;








    protected async override void Start()
    {
        //��鰴ť����Ƿ����
        if (PlayButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the MainMenuPanel.");
            return;
        }

        //����ť�ͺ���������
        PlayButton.onClick.AddListener(() => PlayGame() );
        QuitButton.onClick.AddListener(() => QuitGame() );


        //Ĭ�ϰ�ťΪ����ʼ��Ϸ����ť����������õ�EventSystem
        lastSelectedButton = PlayButton.gameObject;


        base.Start();

        //����������BGN
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.MyVeryOwnDeadShip, true, SoundManager.Instance.MusicVolume);
    }







    private async void PlayGame()
    {
        //����һ¥��������
        SceneManager.LoadScene("FirstFloor");

        //����һ¥BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);

        ClosePanel();
    }

    private void QuitGame()
    {
        //�˳���Ϸ�����ڱ�����Ϸ����
        Application.Quit();

        //��Unity�༭�����˳�����ģʽ
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}