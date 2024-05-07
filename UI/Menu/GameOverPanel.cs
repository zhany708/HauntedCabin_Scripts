using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameOverPanel : PanelWithButton
{
    public Button RestartButton;
    public Button QuitButton;





    protected override void Awake()
    {
        base.Awake();

        //Ĭ�ϰ�ťΪ�����¿�ʼ����ť
        firstSelectedButton = RestartButton.gameObject;

        //���ô˽���ĵ���ֵ
        FadeInAlpha = 0.75f;
    }

    private void Start()
    {
        //��鰴ť����Ƿ����
        if (RestartButton == null || QuitButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the GameOverPanel.");
            return;
        }

        //����ť�ͺ���������
        RestartButton.onClick.AddListener(() => OnRestartButtonClick());
        QuitButton.onClick.AddListener(() => OnQuitButtonClick());
    }

    
    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += HandleFadeOutFinished;        //���׵���ʱִ�к���
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= HandleFadeOutFinished;
    }
    






    private void OnRestartButtonClick()
    {
        //�رս���
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }
   
    private void OnQuitButtonClick()
    {
        //�˳���Ϸ�����ڱ�����Ϸ����
        Application.Quit();

        //��Unity�༭�����˳�����ģʽ
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }



    private void HandleFadeOutFinished()
    {
        //�������˵�
        SceneManager.LoadScene("MainMenu");

        //������Ϸ�ĸ���ϵͳ
        ResetGameSystems();
    }

    private void ResetGameSystems()
    {
        EventManager.Instance.ResetGame();
        EnemyPool.Instance.ResetGame();
    }
}