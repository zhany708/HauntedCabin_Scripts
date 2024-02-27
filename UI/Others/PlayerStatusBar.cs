using UnityEngine;
using UnityEngine.UI;



public class PlayerStatusBar : BasePanel
{
    public Image HpImage;
    public Image HpEffectImage;     //血量缓冲图片



    HealthBar m_PlayerHealthBar;





    protected override void Awake()
    {
        GameObject playerHealthBar = GameObject.Find("PlayerHealthBar");    //获取PlayerHealthBar游戏物体

        if (playerHealthBar != null )
        {
            m_PlayerHealthBar = playerHealthBar.GetComponent<HealthBar>();       //获取脚本组件
        }


        if (m_PlayerHealthBar != null )
        {
            m_PlayerHealthBar.SetHpImage(HpImage);
            m_PlayerHealthBar.SetHpEffectImage(HpEffectImage);
        }        
    }
}
