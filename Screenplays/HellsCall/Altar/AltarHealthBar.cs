using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;



public class AltarHealthBar : HealthBar      //用于祷告石的血条控制
{
    Altar m_Altar;







    private void Awake() 
    {
        //获取Player组件
        m_Altar = FindAnyObjectByType<Altar>();
        if (m_Altar == null)
        {
            Debug.LogError("Altar component not found in the " + name);
            return;
        }
    }

    protected override void Start()
    {
        //初始化最大生命值等变量后，随后再调用父类中的Start函数
        //InitializeAltarHealthBarAsync();       //Start等Unity内部函数调用异步加载函数时，无需使用await

        base.Start();
    }
    

    /*
    //因为需要异步加载UI。所以使用async（如果不使用的话，可能会出现还没加载完就接着跑下面的代码的情况）
    private async void InitializeAltarHealthBarAsync()   
    {       
        //检查UIKeys是否为空且要加载的名字是否存在，随后等待UI加载完毕
        if (UIManager.Instance.UIKeys != null && !string.IsNullOrEmpty(UIManager.Instance.UIKeys.AltarHealthBarPanel))
        {
            //检查界面是否已经打开,没有的话则打开界面
            if (!UIManager.Instance.PanelDict.ContainsKey(UIManager.Instance.UIKeys.AltarHealthBarPanel) )
            {
                await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.AltarHealthBarPanel);    //打开祷告石血条
            }

            //如果已经打开的话,则报错
            else
            {
                Debug.LogError("AltarHealthBarPanel is already openend.");
                return;
            }
        }

        else
        {
            Debug.LogError("UIKeys not set or the key for AltarHealthBarPanel is empty.");
            return;
        }
        

        //游戏开始时初始化最大生命值
        SetMaxHp(m_Altar.GetMaxHealth);
    }
    */
}