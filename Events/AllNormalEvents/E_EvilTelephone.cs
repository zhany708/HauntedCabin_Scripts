using UnityEngine;



public class E_EvilTelephone : Event    //E开头的脚本表示跟事件相关
{
    Animator m_Animator;
    AudioSource m_AudioSource;
    Collider2D m_Collider;




    protected override void Awake()
    {
        base.Awake();

        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Collider = GetComponent<Collider2D>();

        if (m_Animator == null || m_AudioSource == null || m_Collider == null)
        {
            Debug.LogError("One or more components are missing on " + gameObject.name);
            return;
        }
    }

    private void OnEnable()
    {
        //每次加载时都重置碰撞框
        m_Collider.enabled = true;

        EvilTelephonePanel.OnResultFinished += FinishEvent;     //UI界面关闭后再执行事件结束逻辑
    }

    private void OnDisable()
    {
        EvilTelephonePanel.OnResultFinished -= FinishEvent;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerPlayerInteraction(other);    //跟玩家交互的逻辑
        }      
    }


    private void OnDestroy()
    {       
        //释放响铃声的音频
        SoundManager.Instance.ReleaseAudioClip(EventData.AudioClipNames[0]);
        //释放接电话的音频
        SoundManager.Instance.ReleaseAudioClip(EventData.AudioClipNames[1]);      
    }




    public override void StartEvent()
    {
        m_Animator.SetBool("Ringing", true);
    }



    private async void TriggerPlayerInteraction(Collider2D playerCollider)
    {
        m_Animator.SetBool("Ringing", false);       //角色触碰电话后取消震动

        PlayAnswerPhoneSound();     //播放接电话的音效

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.EvilTelephonePanel);   //打开事件界面

        m_Collider.enabled = false;     //界面打开后，取消激活碰撞框
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

    #region AnimationEvents
    private void PlayRingSound()        //用于动画帧事件，播放响铃声
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)      //防止重复播放
        {
            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[0], 0.6f);
        }
    }
    #endregion    
}