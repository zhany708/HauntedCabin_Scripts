using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    public SO_UIKeys UIKeys;



    Image m_HpImage;
    Image m_HpEffectImage;     //Ѫ������ͼƬ

    Player m_Player;


    float m_MaxHp;
    float m_CurrentHp;
    float m_BuffTime = 0.5f;    //����ʱ��

    Coroutine m_UpdateCoroutine;    //��ֹ��һ��Э�̻�û�����Ϳ�ʼ�µ�Э�̣���Ѫ��




    private async void Start()     //��Ϊ��Ҫ�첽����UI������ʹ��async�������ʹ�õĻ������ܻ���ֻ�û������ͽ���������Ĵ���������
    {
        //���UIKeys�Ƿ�Ϊ����Ҫ���ص������Ƿ���ڣ����ȴ�UI�������
        if (UIKeys != null && !string.IsNullOrEmpty(UIKeys.PlayerStatusBarKey) )
        {
            await UIManager.Instance.OpenPanel(UIKeys.PlayerStatusBarKey);    //��ʾ���״̬��
        }

        else
        {
            Debug.LogError("UIKeys not set or playerStatusBarKey is empty.");
        }
        

        //UI������Ϻ�Ż��ȡ���
        m_Player = GetComponentInParent<Player>();

        m_MaxHp = m_Player.PlayerData.MaxHealth;        //��Ϸ��ʼʱ��ʼ���������ֵ
        m_CurrentHp = m_MaxHp;

        //���ʱ�������д˺������Ͳ������ĳ���첽���ص������û��ʼ����Ͼ���Ҫʹ��
        UpdateHealthBar();
    }




    public void SetCurrentHealth(float health)
    {
        m_CurrentHp = Mathf.Clamp(health, 0f, m_MaxHp);     //���ص�ֵ������0��Ѫ������֮��
        UpdateHealthBar();
    }


    private void UpdateHealthBar()
    {
        //ʹ�����ǰ����Ƿ�Ϊ��
        if (m_HpImage == null || m_HpEffectImage == null)
        {
            Debug.LogError("Health bar images are not set.");
            return;
        }


        m_HpImage.fillAmount = m_CurrentHp / m_MaxHp;

        if (m_UpdateCoroutine != null)      
        {
            StopCoroutine(m_UpdateCoroutine);     //���Э�����ڽ��У���ֹͣ��Ȼ��ʼ�µ�Э�̣���ֻ֤��һ��Э�̴���
        }

        m_UpdateCoroutine = StartCoroutine(UpdateHpEffect());
    }





    private IEnumerator UpdateHpEffect()
    {
        float effectLength = m_HpEffectImage.fillAmount - m_HpImage.fillAmount;     //�����Ѫ��
        float elapsedTime = 0f;     //����ȷ������ʱ����0.5����

        while (elapsedTime < m_BuffTime && effectLength != 0f)
        {
            elapsedTime += Time.deltaTime;
            m_HpEffectImage.fillAmount = Mathf.Lerp(m_HpImage.fillAmount + effectLength, m_HpImage.fillAmount, elapsedTime/m_BuffTime);   //����ֵ���ݵ��������������� 0�򷵻ز���һ��1�򷵻ز�������0.5�򷵻��е�

            yield return null;      //�ȴ�һ֡��ʱ��
        }

        m_HpEffectImage.fillAmount = m_HpImage.fillAmount;      //��ֹ����Ѫ��������ɫѪ��
    }


    #region Setters
    public void SetHpImage(Image thisImage)
    {
        m_HpImage = thisImage;
    }

    public void SetHpEffectImage(Image thisImage)
    {
        m_HpEffectImage = thisImage;
    }
    #endregion
}
