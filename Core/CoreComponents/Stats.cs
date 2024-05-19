using System;
using System.Collections;
using UnityEngine;
using ZhangYu.Utilities;



public class Stats : CoreComponent      //���ڹ���������ħ����״̬��Ϣ
{
    public event Action OnHealthZero;       //���շ�ΪDeath�ű�

    public float MaxHealth { get; private set; }



    protected float currentHealth;

    float m_Defense;
    float m_DefenseRate = 0.01f;     //ÿһ�������Ӧ1%���˺�����






    private void Start()
    {
        MaxHealth = core.MaxHealth;     //��Core�����ò���
        m_Defense = core.Defense;

        currentHealth = MaxHealth;      //��Ϸ��ʼʱ���õ�ǰ����ֵ
    }




    public virtual void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);    //ȷ������ֵ���ᳬ���������
    }

    public virtual void DecreaseHealth(float amount, bool doesIgnoreDefense)
    {
        if (currentHealth != 0)      //����ֵΪ0ʱ�Ͳ������������
        {
            if (doesIgnoreDefense)      //���ӷ���
            {
                
                currentHealth -= amount;
            }
            else      //���ݷ����������ܵ����˺�
            {
                currentHealth -= (amount * GetDefenseAddition());      
            }
            
            

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                OnHealthZero?.Invoke();     //�ȼ���Ƿ�Ϊ�գ��ٵ�����ʱ����

                //Debug.Log("Health is zero!!");
            }
        }
    }


    //������ʱ�����ţ���һ������Ϊ����ʱ�䡣�ڶ���Ϊ�˺�ֵ��������ΪƵ�ʣ�
    public IEnumerator HealthDrain(float duration, float damageAmount, float frequency)        
    {
        Timer timer = new Timer(duration);

        StartCoroutine(timer.WaitForDuration() );       //��ʼ��ʱ

        while(!timer.GetIsTimerDone() )
        {
            yield return new WaitForSeconds(60 / frequency);        //ÿ60/Ƶ�ʣ���λ���룩����һ��Ѫ��
            DecreaseHealth(damageAmount, true);       //����Ѫ�������ӷ�����
        }
    }




    private float GetDefenseAddition()   //ÿ���۳�Ѫ��ʱ����Ҫ���ô˺���
    {
        return 1 - m_Defense * m_DefenseRate;       //�����˺�����
    }

    #region Getters
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    #endregion

    #region Setters
    public void SetCurrentHealth(float health)
    {
        currentHealth = health;
    }
    #endregion
}