using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class AltarHealthBarPanel : BasePanel
{
    public Vector3 offset;                          // Offset for the health bar position relative to the player



    Transform m_AltarTransform;                     //
    RectTransform m_HealthBarRectTransform;
    Camera m_MainCamera;





    void Start()
    {
        m_HealthBarRectTransform = GetComponent<RectTransform>();
        m_MainCamera = Camera.main;
    }

    void Update()
    {
        // Convert the player's world position to screen space
        Vector3 screenPosition = m_MainCamera.WorldToScreenPoint(m_AltarTransform.position + offset);

        // Update the health bar's position
        m_HealthBarRectTransform.position = screenPosition;
    }
}
