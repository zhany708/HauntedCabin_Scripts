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
    }

    protected override void Start()     
    {
        //初始化最大生命值等变量后，随后再调用父类中的Start函数
        InitializePlayerHealthBarAsync();       //Start等Unity内部函数调用异步加载函数时，无需使用await

        base.Start();
    }
    #endregion


    #region 血条相关
    //因为需要异步加载UI。所以使用async（如果不使用的话，可能会出现还没加载完就接着跑下面的代码的情况）
    private void InitializePlayerHealthBarAsync()   
    {       
        //检查UIKeys是否为空且要加载的名字是否存在，随后等待UI加载完毕
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.PlayerStatusBarKey))
        {
            //检查界面是否已经打开,打开的话则进行以下逻辑
            if (UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.PlayerStatusBarKey) )
            {
                //Debug.Log("PlayerStatusBarKey again set the images to the " + name);

                PlayerStatusBar.Instance.SetImagesToHealthBar();      //重新赋值图片

                //这里重新加载时需要用协程，否则会出现重新加载后无法正常显示数值的情况
                StartCoroutine(PlayerStatusBar.Instance.DelayedUpdateStatusUI() );
            }
        }
        //如果查找不到界面对应的Key
        else
        {
            Debug.LogError("UIKeys not set or PlayerStatusBarKey is empty.");
            return;
        }
        

        //游戏开始时初始化最大生命值
        SetMaxHp(m_Player.PlayerData.MaxHealth);
    }




    public override void SetCurrentHealth(float health)
    {
        base.SetCurrentHealth(health);

        OnHealthChange?.Invoke();           //调用事件
    }
    #endregion
}