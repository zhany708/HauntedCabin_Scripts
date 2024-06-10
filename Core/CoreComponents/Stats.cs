using System;
using System.Collections;
using UnityEngine;
using ZhangYu.Utilities;



public class Stats : CoreComponent      //用于管理生命，魔力等状态信息
{
    public event Action OnHealthZero;       //接收方为Death脚本

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }


    float m_Defense;
    float m_DefenseRate = 0.01f;     //每一点防御对应1%的伤害减免






    private void Start()
    {
        MaxHealth = core.MaxHealth;     //从Core那里获得参数
        m_Defense = core.Defense;

        CurrentHealth = MaxHealth;      //游戏开始时重置当前生命值
    }




    public virtual void IncreaseHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);    //确保生命值不会超过最大上限
    }

    public virtual void DecreaseHealth(float amount, bool doesIgnoreDefense)
    {
        if (CurrentHealth != 0)      //生命值为0时就不会继续受伤了
        {
            if (doesIgnoreDefense)      //无视防御
            {              
                CurrentHealth -= amount;
            }
            else      //根据防御力减免受到的伤害
            {
                CurrentHealth -= (amount * GetDefenseAddition());      
            }
            
            

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;

                OnHealthZero?.Invoke();     //先检查是否为空，再调用延时函数

                //Debug.Log("Health is zero!!");
            }
        }
    }


    //生命随时间流逝（第一个参数为持续时间，第二个为伤害值，第三个为频率）
    public IEnumerator HealthDrain(float duration, float damageAmount, float frequency)        
    {
        Timer timer = new Timer(duration);

        StartCoroutine(timer.WaitForDuration() );       //开始计时

        while(!timer.GetIsTimerDone() )
        {
            yield return new WaitForSeconds(60 / frequency);        //每60/频率（单位：秒）掉落一次血量
            DecreaseHealth(damageAmount, true);       //掉落血量（无视防御）
        }
    }




    private float GetDefenseAddition()   //每当扣除血量时都需要调用此函数
    {
        return 1 - m_Defense * m_DefenseRate;       //计算伤害减免
    }


    #region Getters
    public float GetCurrentHealth()
    {
        return CurrentHealth;
    }
    #endregion


    #region Setters
    public void SetCurrentHealth(float health)
    {
        CurrentHealth = health;
    }
    #endregion
}