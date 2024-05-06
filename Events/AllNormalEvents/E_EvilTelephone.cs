using UnityEngine;



public class E_EvilTelephone : Event
{
    Animator m_Animator;
    AudioSource m_AudioSource;
    Collider2D m_Collider;




    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        //每次加载时都重置碰撞框
        m_Collider.enabled = true;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_Animator.SetBool("Ringing", false);       //角色触碰电话后取消震动

            PlayAnswerPhoneSound();     //播放接电话的音效

            PlayerStats playerHealth = other.GetComponentInParent<Player>().GetComponentInChildren<PlayerStats>();    //因为进入触发器的是Core里的Combat，所以先获取父物体，再获取子物体

            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(20);
                m_Collider.enabled = false;     //玩家受伤后，取消激活碰撞框
            }
        }

        FinishEvent();      //结束事件
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




    private void PlayRingSound()        //用于动画帧事件，播放响铃声
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)      //防止重复播放
        {
            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[0], 0.6f);
        }
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
}