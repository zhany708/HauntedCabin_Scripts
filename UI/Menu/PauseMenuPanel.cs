using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PauseMenuPanel : PanelWithButton       //������Ϸ�����ж����õ��˽���
{
    public static PauseMenuPanel Instance { get; private set; }

    public Button ResumeButton;
    public Button MainMenuButton;
    public Button QuitButton;


    //��static�������ô˽ű������ü��ɵ��ô˱���
    public static bool IsGamePaused {  get; private set; }







    protected override void Awake()
    {
        base.Awake();

        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //ֻ����û�и�����ʱ�����з�ɾ������������������
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }




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
        FadeDuration = 0;
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

    //��д��������Ϊ�˽�����Ϸ��ʼʱ�ʹ���
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







    //�ָ���Ϸ
    private void Resume()
    {
        //base.OnDisable();

        //�رս���
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);

        //�ָ���Ϸ��ʱ������
        Time.timeScale = 1f;
        
        IsGamePaused = false;
    }


    //��ͣ��Ϸ
    private void Pause()
    {
        //base.OnEnable();

        //�򿪽���
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);

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
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            //��Ϸ��ʼʱ���ؿ�ʼ����
            Debug.Log("You are already in the Main Menu!");
        }

        else
        {
            //EventSystem.current.gameObject.SetActive(false);

            //�������˵�ʱ���򷵻����˵�
            SceneManager.LoadScene("MainMenu");
        }

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