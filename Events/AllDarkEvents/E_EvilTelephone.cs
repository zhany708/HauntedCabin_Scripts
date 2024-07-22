using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class E_EvilTelephone : DarkEvent
{
    Animator m_Animator;
    AudioSource m_AudioSource;


    float m_RingingVolume = 0.6f;       //电话响铃的声音大小





    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();

        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();

        if (m_Animator == null || m_AudioSource == null)
        {
            Debug.LogError("One or more components are missing on " + gameObject.name);
            return;
        }
    }

    private async void OnEnable()
    {
        EvilTelephonePanel.OnResultFinished += FinishEvent;     //UI界面关闭后再执行事件结束逻辑

        await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.EvilTelephonePanel);   //提前加载事件界面



        //提前加载事件所需的所有音效
        for (int i = 0; i < EventData.AudioClipNames.Count; i++)
        {
            //Debug.Log("This audio is loaded: " + EventData.AudioClipNames[i]);

            await SoundManager.Instance.LoadClipAsync(EventData.AudioClipNames[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //打开互动面板
            UIManager.Instance.OpenInteractPanel(() => TriggerPlayerInteraction());
        }
    }

    //由于该界面强制玩家选择，没有提供取消选择的机会，因此无需在OnTriggerStay2D中再次打开

    private void OnTriggerExit2D(Collider2D other)
    {
        //检查是否是玩家碰撞，再检查界面是否存于字典中，最后再检查界面是否打开
        if (other.CompareTag("Player") && UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.InteractPanel)
            && !InteractPanel.Instance.IsRemoved)
        {
            UIManager.Instance.ClosePanel(UIManager.Instance.UIKeys.InteractPanel, true);      //淡出互动界面
        }
    }

    private void OnDisable()
    {
        EvilTelephonePanel.OnResultFinished -= FinishEvent;
    }

    private void OnDestroy()
    {
        //释放事件的所有音效
        for (int i = 0; i < EventData.AudioClipNames.Count; i++)
        {
            SoundManager.Instance.ReleaseAudioClip(EventData.AudioClipNames[i]);
        }
    }
    #endregion


    #region 事件相关
    public override void StartEvent()
    {
        m_Animator.SetBool("Ringing", true);
    }



    private async void TriggerPlayerInteraction()
    {
        //先设置互动界面里的布尔，防止重复调用
        InteractPanel.Instance.SetIsActionCalled(true);



        PlayAnswerPhoneSound();                     //播放接电话的音效

        m_Animator.SetBool("Ringing", false);       //角色触碰电话后取消震动

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.EvilTelephonePanel);   //打开事件界面
    }



    private void PlayAnswerPhoneSound()     //播放接电话的音效
    {
        if (m_AudioSource != null)
        {
            //先暂停当前的音频
            m_AudioSource.Stop();

            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[1], EventData.AudioVolume);
        }
    }
    #endregion


    #region 动画帧事件
    private void PlayRingSound()        //用于动画帧事件，播放响铃声
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)      //防止重复播放
        {
            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[0], m_RingingVolume);
        }
    }
    #endregion    
}
