using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HellsCall : BaseScreenplay
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






    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //�������ʱֹͣЭ��
    }

    private void OnDisable()
    {
        PlayerStats.OnHealthZero -= DestroyCoroutine;
    }

   



    public async override void StartScreenplay()
    {

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //�򿪾籾��������

        /*
        if (PlayerStats != null)
        {
            //����x�룬ÿ�ε�x��Ѫ��Ƶ��Ϊx
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(60f, 10f, 12f));
        }
        */
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