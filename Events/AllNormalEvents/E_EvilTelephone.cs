using UnityEngine;



public class E_EvilTelephone : Event    //E��ͷ�Ľű���ʾ���¼����
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
        //ÿ�μ���ʱ��������ײ��
        m_Collider.enabled = true;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerPlayerInteraction(other);    //����ҽ������߼�
            FinishEvent();      //�����¼�
        }      
    }


    private void OnDestroy()
    {       
        //�ͷ�����������Ƶ
        SoundManager.Instance.ReleaseAudioClip(EventData.AudioClipNames[0]);
        //�ͷŽӵ绰����Ƶ
        SoundManager.Instance.ReleaseAudioClip(EventData.AudioClipNames[1]);      
    }




    public override void StartEvent()
    {
        m_Animator.SetBool("Ringing", true);
    }



    private void TriggerPlayerInteraction(Collider2D playerCollider)
    {
        m_Animator.SetBool("Ringing", false);       //��ɫ�����绰��ȡ����

        PlayAnswerPhoneSound();     //���Žӵ绰����Ч

        //��Ϊ���봥��������Core���Combat�������Ȼ�ȡ�����壬�ٻ�ȡ������
        PlayerStats playerHealth = playerCollider.GetComponentInParent<Player>().GetComponentInChildren<PlayerStats>();    

        if (playerHealth != null)
        {
            playerHealth.DecreaseHealth(20);
        }

        m_Collider.enabled = false;     //������˺�ȡ��������ײ��
    }



    private void PlayAnswerPhoneSound()     //���Žӵ绰����Ч
    {
        if (m_AudioSource != null)
        {
            //����ͣ��ǰ����Ƶ
            m_AudioSource.Stop();

            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[1], EventData.AudioVolume);
        }
    }

    #region AnimationEvents
    private void PlayRingSound()        //���ڶ���֡�¼�������������
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)      //��ֹ�ظ�����
        {
            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[0], 0.6f);
        }
    }
    #endregion    
}