using UnityEngine;
using UnityEngine.UI;



public class MainMenuPanel : PanelWithButton
{
    public Button PlayButton;       //��ʼ��Ϸ��ť
    public Button SettingButton;    //���ý��水ť
    public Button QuitButton;       //�ر���Ϸ��ť







    protected override void Awake()
    {
        base.Awake();

        //��鰴ť����Ƿ����
        if (PlayButton == null || SettingButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the MainMenuPanel.");
            return;
        }

        //Ĭ�ϰ�ťΪ����ʼ��Ϸ����ť
        firstSelectedButton = PlayButton.gameObject;
    }


    private async void Start()
    {
        //����ť�ͺ���������
        PlayButton.onClick.AddListener(() => PlayGame());
        SettingButton.onClick.AddListener(() => OpenSettingPanel());
        QuitButton.onClick.AddListener(() => QuitGame());

        
        //��ʼ����������
        if (panelName  == null)
        {
            panelName = UIManager.Instance.UIKeys.MainMenuPanel;
        }
        

        //����������BGN
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.MyVeryOwnDeadShip, true);
    }






    private async void PlayGame()
    {
        //����Ϸ�������ܽ���
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.GameBackgroundPanel);

        //������ǰ����
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }

    private async void OpenSettingPanel()
    {
        //����Ϸ���ý���
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.SettingPanel);
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