using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PauseMenuPanel : PanelWithButton       //������Ϸ�����ж����õ��˽���
{
    public static PauseMenuPanel Instance { get; private set; }

    

    public Button ResumeButton;
    public Button MainMenuButton;
    public Button QuitButton;



    bool m_IsGamePaused = false;       //��ʾ��Ϸ�Ƿ�����ͣ״̬





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

        //Ĭ�ϰ�ťΪ���ָ�����ť
        firstSelectedButton = ResumeButton.gameObject;


        //���ô˽���ĵ���ֵ
        FadeInAlpha = 0.75f;
        FadeDuration = 0;
    }


    private void Start()
    {
        //����ť�ͺ���������
        ResumeButton.onClick.AddListener(() => Resume());
        MainMenuButton.onClick.AddListener(() => BackToMainMenu());
        QuitButton.onClick.AddListener(() => QuitGame());


        //���õ�ǰ���������
        panelName = UIManager.Instance.UIKeys.PauseMenuPanel;      
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
            if (m_IsGamePaused)
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
        //�رս���
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);

        //�ָ���Ϸ��ʱ������
        Time.timeScale = 1f;
        
        m_IsGamePaused = false;
        SetBothMoveableAndAttackable(true);     //��������ƶ��͹���
    }


    //��ͣ��Ϸ
    private void Pause()
    {
        //�򿪽���
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);

        //��ͣ��Ϸ��ʱ������
        Time.timeScale = 0f;        

        m_IsGamePaused = true;
        SetBothMoveableAndAttackable(false);    //����������ƶ��͹���
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
            //�������˵�ʱ���򷵻����˵�
            SceneManager.LoadScene("MainMenu");

            //������Ϸ�ĸ���ϵͳ
            ResetGameSystems();
        }
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