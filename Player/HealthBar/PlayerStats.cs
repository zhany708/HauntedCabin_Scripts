public class PlayerStats : Stats
{
    HealthBar m_PlayerHealthBar;

    
    protected override void Awake()
    {
        base.Awake();

        m_PlayerHealthBar = GetComponentInChildren<HealthBar>();      //获取血条组件的血条缓冲脚本
    }




    public override void DecreaseHealth(float amount, bool doesIgnoreDefense)
    {
        base.DecreaseHealth(amount, doesIgnoreDefense);

        m_PlayerHealthBar.SetCurrentHealth(currentHealth);      //调用血条脚本中的更新生命值函数
    }

    public override void IncreaseHealth(float amount)
    {
        base.IncreaseHealth(amount);

        m_PlayerHealthBar.SetCurrentHealth(currentHealth);
    }
}