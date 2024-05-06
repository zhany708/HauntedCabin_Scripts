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
        RestartButton.onClick.AddListener(() => Restart());
        QuitButton.onClick.AddListener(() => QuitGame());
    }

    
    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += BackToMainMenu;        //���׵���ʱִ�з������˵��ĺ���
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= BackToMainMenu;
    }
    






    private void Restart()
    {
        //�رս���
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }

    //�������˵�
    private void BackToMainMenu()
    {
        //�������˵�
        SceneManager.LoadScene("MainMenu");

        //������Ϸ�ĸ���ϵͳ
        EventManager.Instance.ResetGame();
        EnemyPool.Instance.ResetGame();       
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