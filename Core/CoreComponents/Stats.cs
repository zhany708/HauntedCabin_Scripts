using System;
using UnityEngine;



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

    public virtual void DecreaseHealth(float amount)
    {
        if (currentHealth != 0)      //����ֵΪ0ʱ�Ͳ������������
        {
            currentHealth -= (amount * GetDefenseAddition() );      //���ݷ����������ܵ����˺�
            

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                OnHealthZero?.Invoke();     //�ȼ���Ƿ�Ϊ�գ��ٵ�����ʱ����

                //Debug.Log("Health is zero!!");
            }
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
