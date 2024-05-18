using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ScreenplayManager : ManagerTemplate<ScreenplayManager>
{
    public PlayerStats PlayerStats      //Lazy load
    {
        get
        {
            if (m_PlayerStats == null)
            {
                m_PlayerStats = FindObjectOfType<PlayerStats>();
            }
            return m_PlayerStats;
        }
    }
    private PlayerStats m_PlayerStats;



    Coroutine m_HealthDrainCoroutine;






    private void Start()
    {
        if (PlayerStats != null)
        {
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(60f, 10f, 12f));
        }        
    }


    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //�������ʱֹͣЭ��
    }

    private void OnDisable()
    {
        PlayerStats.OnHealthZero -= DestroyCoroutine;
    }






    private void DestroyCoroutine()     //ֹͣЭ��
    {
        if (m_HealthDrainCoroutine != null)
        {
            Debug.Log("Coroutine stopped!!");
            StopCoroutine(m_HealthDrainCoroutine);
        }
    }
}