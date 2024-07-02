using UnityEngine;
using System;




public class PlayerHealthBar : HealthBar      //用于玩家的血条控制
{
    public event Action OnHealthChange;       //接收方为PlayerStatusBar


    Player m_Player;






    #region Unity内部函数
    private void Awake() 
    {
        //获取Player组件
        m_Player = GetComponentInParent<Player>();
        if (m_Player == null)
        {
            Debug.LogError("Player component not found in the parent of " + name);
            return;
        }

        //游戏开始时初始化最大生命值
        SetMaxHp(m_Player.PlayerData.MaxHealth);
    }
    #endregion


    #region 血条相关
    public override void SetCurrentHealth(float health)
    {
        base.SetCurrentHealth(health);

        OnHealthChange?.Invoke();           //调用事件
    }
    #endregion
}