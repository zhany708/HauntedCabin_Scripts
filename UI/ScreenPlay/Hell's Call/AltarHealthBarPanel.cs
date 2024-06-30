using UnityEngine.UI;
using UnityEngine;

/*
 * Introduction：仪式台的血条UI
 * Creator：Zhang Yu
*/

public class AltarHealthBarPanel : BasePanel
{
    //传递给HealthBar脚本的照片
    public Image HpImage;
    public Image IncreaseHpEffectImage;
    public Image DecreaseHpEffectImage;



    Transform m_AltarTransform;                     //仪式台的位置信息




    protected override void Awake()
    {
        m_AltarTransform = GetComponentInParent<Altar>().transform;

        SetImagesToHealthBar();
    }

    private void Start()
    {
        //赋值界面名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.HellsCall_AltarHealthBarPanel;
        }

        //添加缓存进字典，表示界面正在打开
        UIManager.Instance.PanelDict[panelName] = this;

        //初始化时设置界面的透明度（隐藏界面）
        CanvasGroup.alpha = FadeOutAlpha;
    }




    public void SetImagesToHealthBar()      //用于将照片组件传递给玩家血条
    {
        //获取玩家血条的脚本组件
        AltarHealthBar m_AltarHealthBar = m_AltarTransform.GetComponentInChildren<AltarHealthBar>();
        if (m_AltarHealthBar == null)
        {
            Debug.LogError("AltarHealthBar component not found under Altar object.");
            return;
        }

        //传递血条和缓冲图片
        m_AltarHealthBar.SetHpImage(HpImage);
        m_AltarHealthBar.SetIncreaseHpEffectImage(IncreaseHpEffectImage);
        m_AltarHealthBar.SetDecreaseHpEffectImage(DecreaseHpEffectImage);
    }
}