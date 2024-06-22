using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;



public class PlayerHealthBar : HealthBar      //用于玩家的血条控制
{
    Player m_Player;







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
    


    //因为需要异步加载UI。所以使用async（如果不使用的话，可能会出现还没加载完就接着跑下面的代码的情况）
    private async void InitializePlayerHealthBarAsync()   
    {       
        //检查UIKeys是否为空且要加载的名字是否存在，随后等待UI加载完毕
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.PlayerStatusBarKey))
        {
            //检查界面是否已经打开,没有的话则打开界面
            if (!UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.PlayerStatusBarKey) )
            {
                await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.PlayerStatusBarKey);    //打开玩家状态栏
            }

            //如果已经打开的话,则重新传递照片组件给当前脚本
            else
            {
                PlayerStatusBar.Instance.SetImagesToHealthBar();
            }
        }

        else
        {
            Debug.LogError("UIKeys not set or PlayerStatusBarKey is empty.");
        }
        

        //游戏开始时初始化最大生命值
        SetMaxHp(m_Player.PlayerData.MaxHealth);
    }
}