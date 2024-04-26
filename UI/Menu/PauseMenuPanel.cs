using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PauseMenuPanel : PanelWithButton       //������Canvas��Ϸ�����ϣ���Ϊ������Ϸ�����ж��п��ܻ��õ��˽���
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


        if (lastSelectedButton == null)
        {
            //Ĭ�ϰ�ťΪ����ʼ��Ϸ����ť����������õ�EventSystem
            lastSelectedButton = ResumeButton.gameObject;
        }         
    }


    protected override void Start()
    {
        base.Start();

        panelName = UIManager.Instance.UIKeys.PauseMenuPanel;

        //��UI����ӽ��ֵ�
        if (!UIManager.Instance.PanelDict.ContainsKey(panelName) )
        {
            UIManager.Instance.PanelDict.Add(panelName, this);
        }      
    }


    protected override void Update()
    {
        base.Update();

        //����ű������������Ƿ���Esc������ͣ�����а��µĻ���ָ���Ϸ���������ͣ��Ϸ    ��Ҫ���ģ����һ����ʱ������ֹ��ҷ�����Escʱ����Ƶ���Ĵ򿪺͹ر�
        if (playerInputHandler.IsEscPressed)
        {
            //��Ϸ��ͣʱ
            if (isGamePaused)
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
        PauseMenuUI.SetActive(false);

        //�ָ���Ϸ��ʱ������
        Time.timeScale = 1f;
      
        isGamePaused = false;
    }

    //��ͣ��Ϸ
    private void Pause()
    {
        PauseMenuUI.SetActive(true);

        //��ͣ��Ϸ��ʱ������
        Time.timeScale = 0f;

        isGamePaused = true;
    }





    private void BackToMainMenu()
    {
        //�Ȼָ���Ϸ����󷵻����˵�
        Resume();

        //���������泡��
        SceneManager.LoadScene("MainMenu");

        //ClosePanel();
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