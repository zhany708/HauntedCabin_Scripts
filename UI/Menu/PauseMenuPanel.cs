using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PauseMenuPanel : PanelWithButton       //������Canvas��Ϸ�����ϣ���Ϊ������Ϸ�����ж��п��ܻ��õ��˽���
{
    public Button ResumeButton;
    public Button MainMenuButton;
    public Button QuitButton;


    //��static�������ô˽ű������ü��ɵ��ô˱���
    public static bool IsGamePaused {  get; private set; }







    protected override void Awake()
    {
        base.Awake();

        //��鰴ť����Ƿ����
        if (ResumeButton == null || MainMenuButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the PauseMenuPanel.");
            return;
        }


        //����ť�ͺ���������
        ResumeButton.onClick.AddListener(() => Resume());
        MainMenuButton.onClick.AddListener(() => BackToMainMenu());
        QuitButton.onClick.AddListener(() => QuitGame());


        //Ĭ�ϰ�ťΪ���ָ�����ť
        firstSelectedButton = ResumeButton.gameObject;
    }


    private void Start()
    {
        //���õ�ǰ���������
        panelName = UIManager.Instance.UIKeys.PauseMenuPanel;

        //��ʼ������
        IsGamePaused = false;

        //���ô˽���ĵ���ֵ
        FadeInAlpha = 0.75f;
    }


    protected override void Update()
    {
        //�������ʱ�ŵ��ø����Update����
        if (CanvasGroup.alpha == FadeInAlpha)
        {
            base.Update();

        }


        //�����������Ƿ���Esc������ͣ�����а��µĻ���ָ���Ϸ���������ͣ��Ϸ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //��Ϸ��ͣʱ
            if (IsGamePaused)
            {
                Resume();
            }

            //��Ϸ����ʱ
            else
            {
                Pause();
            }
        }                  
    }





    //�ָ���Ϸ
    private void Resume()
    {
        base.OnDisable();

        //�رս���
        Fade(CanvasGroup, FadeOutAlpha, 0, false);

        //�ָ���Ϸ��ʱ������
        Time.timeScale = 1f;
        
        IsGamePaused = false;
    }


    //��ͣ��Ϸ
    private void Pause()
    {
        base.OnEnable();        //Bug�������������ʱ���򿪴˽�����޷�������븸���е��б�

        //�򿪽���
        Fade(CanvasGroup, FadeInAlpha, 0, true);

        //��ͣ��Ϸ��ʱ������
        Time.timeScale = 0f;        

        IsGamePaused = true;
    }


    //�������˵�
    private void BackToMainMenu()
    {
        //�Ȼָ���Ϸ����󷵻����˵�
        Resume();

        //���������泡��
        SceneManager.LoadScene("MainMenu");

        //ClosePanel();
    }


    //�˳���Ϸ
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