using System.Collections;
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

    public Vector3 offset;                          //界面坐标与仪式台之间的偏差



    Transform m_AltarTransform;                     //仪式台的位置信息
    RectTransform m_HealthBarRectTransform;         //界面的UI坐标
    Camera m_MainCamera;





    protected override void Awake()
    {
        m_AltarTransform = FindAnyObjectByType<Altar>().transform;
        m_HealthBarRectTransform = GetComponent<RectTransform>();
        m_MainCamera = Camera.main;
    }

    private void Start()
    {
        SetImagesToHealthBar();
    }


    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        //需要做的：将界面改成物体坐标（即玩家远离后不会依然显示在屏幕上）

        //将仪式台的世界坐标转换成屏幕坐标
        Vector3 screenPosition = m_MainCamera.WorldToScreenPoint(m_AltarTransform.position + offset);

        //更新界面的坐标（由于仪式台不会移动，因此设置一次坐标后就不用管了）
        m_HealthBarRectTransform.position = screenPosition;
    }



    public void SetImagesToHealthBar()      //用于将照片组件传递给玩家血条
    {
        //获取玩家血条的脚本组件
        AltarHealthBar m_AltarHealthBar = m_AltarTransform.GetComponent<AltarHealthBar>();
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