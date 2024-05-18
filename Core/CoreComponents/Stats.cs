using System;
using UnityEngine;



public class Stats : CoreComponent      //用于管理生命，魔力等状态信息
{
    public event Action OnHealthZero;       //接收方为Death脚本

    public float MaxHealth { get; private set; }



    protected float currentHealth;

    float m_Defense;
    float m_DefenseRate = 0.01f;     //每一点防御对应1%的伤害减免






    private void Start()
    {
        MaxHealth = core.MaxHealth;     //从Core那里获得参数
        m_Defense = core.Defense;

        currentHealth = MaxHealth;      //游戏开始时重置当前生命值
    }




    public virtual void IncreaseHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);    //确保生命值不会超过最大上限
    }

    public virtual void DecreaseHealth(float amount)
    {
        if (currentHealth != 0)      //生命值为0时就不会继续受伤了
        {
            currentHealth -= (amount * GetDefenseAddition() );      //根据防御力减免受到的伤害
            

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                OnHealthZero?.Invoke();     //先检查是否为空，再调用延时函数

                //Debug.Log("Health is zero!!");
            }
        }
    }


    private float GetDefenseAddition()   //每当扣除血量时都需要调用此函数
    {
        return 1 - m_Defense * m_DefenseRate;       //计算伤害减免
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
