using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class AltarHealthBarPanel : BasePanel
{
    public Vector3 offset;                          //界面坐标与仪式台之间的偏差



    Transform m_AltarTransform;                     //仪式台的位置信息
    RectTransform m_HealthBarRectTransform;         //界面的UI坐标
    Camera m_MainCamera;





    private void Awake() 
    {
        m_HealthBarRectTransform = GetComponent<RectTransform>();
        m_MainCamera = Camera.main;
    }




    public override void OpenPanel(string name)
    {
        base.OpenPanel();

        //将仪式台的世界坐标转换成屏幕坐标
        Vector3 screenPosition = m_MainCamera.WorldToScreenPoint(m_AltarTransform.position + offset);

        //更新界面的坐标（由于祷告石不会移动，因此无需持续的更新）
        m_HealthBarRectTransform.position = screenPosition;
    }
}