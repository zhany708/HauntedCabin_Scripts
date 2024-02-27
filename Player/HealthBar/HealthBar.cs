using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    Image m_HpImage;
    Image m_HpEffectImage;     //Ѫ������ͼƬ

    Player m_Player;


    float m_MaxHp;
    float m_CurrentHp;
    float m_BuffTime = 0.5f;    //����ʱ��

    Coroutine m_UpdateCoroutine;    //��ֹ��һ��Э�̻�û�����Ϳ�ʼ�µ�Э�̣���Ѫ��




    private void Start()
    {
        UIManager.Instance.OpenPanel("PlayerStatusBar");    //��ʾ���״̬��

        m_Player = GetComponentInParent<Player>();

        m_MaxHp = m_Player.PlayerData.MaxHealth;        //��Ϸ��ʼʱ��ʼ���������ֵ
        m_CurrentHp = m_MaxHp;

        UpdateHealthBar();
    }




    public void SetCurrentHealth(float health)
    {
        m_CurrentHp = Mathf.Clamp(health, 0f, m_MaxHp);     //���ص�ֵ������0��Ѫ������֮��
        UpdateHealthBar();
    }


    private void UpdateHealthBar()
    {
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
