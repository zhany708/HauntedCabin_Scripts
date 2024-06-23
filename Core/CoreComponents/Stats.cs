using System;
using System.Collections;
using UnityEngine;
using ZhangYu.Utilities;



public class Stats : CoreComponent      //用于管理生命，魔力等状态信息
{
    public event Action OnHealthZero;       //接收方为Death, Altar脚本
    public event Action OnHighHealth;       //接收方为xx
    public event Action OnHalfHealth;       //接收方为Enemy_DefenseWar脚本
    public event Action OnLowHealth;        //接收方为xx

    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }



    HealthBar m_HealthBar;                  //用于控制血条的脚本（默认放在Stats物体的子物体中。没有的话也不影响当前脚本）

    float m_Defense;
    float m_DefenseRate = 0.01f;     //每一点防御对应1%的伤害减免






    protected override void Awake()
    {
        base.Awake();

        m_HealthBar = GetComponentInChildren<HealthBar>();      //获取血条组件的血条缓冲脚本
    }

    private void Start()
    {
        MaxHealth = core.MaxHealth;     //从Core那里获得参数
        m_Defense = core.Defense;

        CurrentHealth = MaxHealth;      //游戏开始时重置当前生命值
    }




    public virtual void IncreaseHealth(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);    //确保生命值不会超过最大上限

        if (m_HealthBar != null)
        {
            m_HealthBar.SetCurrentHealth(CurrentHealth);        //调用血条脚本中的更新生命值函数
        }
    }

    //需要做的：根据当前血量变化调用特定的事件函数，从而让角色根据血量改变一些属性和特点
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
            
                                          
            
            //以下这些条件的顺序很重要，如果顺序反过来的话就永远只会进入一个条件！
            if (CurrentHealth <= 0)     //当血量归零时
            {
                CurrentHealth = 0;

                OnHealthZero?.Invoke();     //先检查是否为空，再调用延时函数

                //Debug.Log("Health is zero!!");
            }

            else if (CurrentHealth <= MaxHealth * 0.33)     //当血量只有三分之一时
            {
                OnLowHealth?.Invoke();      //调用事件函数
            }

            else if (CurrentHealth <= MaxHealth * 0.5)      //当血量只有一半时
            {
                OnHalfHealth?.Invoke();     //调用事件函数
            }

            else if (CurrentHealth <= MaxHealth * 0.66)     //当血量只有三分之二时
            {
                OnHighHealth?.Invoke();     //调用事件函数
            }



            if (m_HealthBar != null)
            {
                m_HealthBar.SetCurrentHealth(CurrentHealth);        //调用血条脚本中的更新生命值函数
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
    public float GetCurrentHelathRate()     //获取当前血量百分比
    {
        return CurrentHealth / MaxHealth;
    }
    #endregion


    #region Setters
    public virtual void SetCurrentHealth(float health)
    {
        CurrentHealth = health;

        if (m_HealthBar != null)
        {
            m_HealthBar.SetCurrentHealth(CurrentHealth);        //调用血条脚本中的更新生命值函数
        }
    }
    #endregion
}