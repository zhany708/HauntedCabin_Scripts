using UnityEngine;



public class PlayerStats : Stats
{
    HealthBar m_PlayerHealthBar;

    
    protected override void Awake()
    {
        base.Awake();

        m_PlayerHealthBar = GetComponentInChildren<HealthBar>();      //获取血条组件的血条缓冲脚本
        if (m_PlayerHealthBar == null)
        {
            Debug.LogError("HealthBar component not found in children.");
            return;
        }
    }




    public override void DecreaseHealth(float amount, bool doesIgnoreDefense)
    {
        base.DecreaseHealth(amount, doesIgnoreDefense);

        m_PlayerHealthBar.SetCurrentHealth(CurrentHealth);      //调用血条脚本中的更新生命值函数
    }

    public override void IncreaseHealth(float amount)
    {
        base.IncreaseHealth(amount);

        m_PlayerHealthBar.SetCurrentHealth(CurrentHealth);
    }


    #region Setters
    public override void SetCurrentHealth(float health)
    {
        base.SetCurrentHealth(health);
        m_PlayerHealthBar.SetCurrentHealth(CurrentHealth);
    }
    #endregion
}