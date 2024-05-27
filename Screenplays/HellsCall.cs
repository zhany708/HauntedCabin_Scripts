using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
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

    int m_StoneNum = 2;     //����ʯ������




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
    }



   


    public void StartHealthDrain()      //��ʼ������Ѫ
    {
        if (PlayerStats != null)
        {
            //����x�룬ÿ�ε�x��Ѫ��Ƶ��Ϊx
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(60f, 10f, 12f));
        }
    }

    private void DestroyCoroutine()     //ֹͣЭ��
    {
        if (m_HealthDrainCoroutine != null)
        {
            UnityEngine.Debug.Log("Coroutine stopped!!");
            StopCoroutine(m_HealthDrainCoroutine);
        }
    }




    public void GenerateMagicStones()       //������������ɵ���ʯ
    {
        int roomNum = RoomManager.Instance.GeneratedRoomDict.Count;     //��ʾ��ǰ�ж��ٷ���

        if (roomNum <= m_StoneNum)      //���������������������е���ʯʱ
        {

        }

        else
        {
            int randomNum = UnityEngine.Random.Range(0, roomNum);       //�����������
        }
    }
}