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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            m_Animator.SetBool("Ringing", false);       //��ɫ�����绰��ȡ����

            PlayAnswerPhoneSound();     //���Žӵ绰����Ч

            PlayerStats playerHealth = other.GetComponentInParent<Player>().GetComponentInChildren<PlayerStats>();    //��Ϊ���봥��������Core���Combat�������Ȼ�ȡ�����壬�ٻ�ȡ������

            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(20);
                m_Collider.enabled = false;     //������˺�ȡ��������ײ��
            }
        }

        FinishEvent();      //�����¼�
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




    private void PlayRingSound()        //���ڶ���֡�¼�������������
    {
        if (m_AudioSource != null && !m_AudioSource.isPlaying)      //��ֹ�ظ�����
        {
            SoundManager.Instance.PlaySFXAsyncWithAudioSource(m_AudioSource, EventData.AudioClipNames[0], 0.6f);
        }
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
}