using UnityEngine;

public class E_EvilTelephone : Event
{
    public AudioClip AnswerClip;
   


    Animator m_Animator;
    AudioSource m_AudioSource;
    Collider2D m_Collider;




    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Collider = GetComponent<Collider2D>();
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






    public override void StartEvent()
    {
        m_Animator.SetBool("Ringing", true);
    }




    private void PlayRingSound()        //用于动画帧事件，播放响铃声
    {
        if (m_AudioSource != null && m_AudioSource.clip != null && !m_AudioSource.isPlaying)    //防止重复播放音效
        {
            m_AudioSource.volume = 0.6f;  //设置音量
            m_AudioSource.Play();   //动画开始时播放响铃声
        }
    }

    private void PlayAnswerPhoneSound()     //播放接电话的音效
    {
        if (m_AudioSource != null)   
        {
            m_AudioSource.clip = AnswerClip;    //更改为接电话的音效

            m_AudioSource.volume = 1f;  //设置到最大音量
            m_AudioSource.Play();   //动画开始时播放响铃声
        }
    }
}
