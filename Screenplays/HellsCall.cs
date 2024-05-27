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

    int m_StoneNum = 2;     //祷告石的数量




    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //玩家死亡时停止协程
    }

    private void OnDisable()
    {
        PlayerStats.OnHealthZero -= DestroyCoroutine;
    }

   



    public async override void StartScreenplay()
    {

        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //打开剧本背景界面       
    }



   


    public void StartHealthDrain()      //开始持续掉血
    {
        if (PlayerStats != null)
        {
            //持续x秒，每次掉x点血，频率为x
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(60f, 10f, 12f));
        }
    }

    private void DestroyCoroutine()     //停止协程
    {
        if (m_HealthDrainCoroutine != null)
        {
            UnityEngine.Debug.Log("Coroutine stopped!!");
            StopCoroutine(m_HealthDrainCoroutine);
        }
    }




    public void GenerateMagicStones()       //在随机房间生成祷告石
    {
        int roomNum = RoomManager.Instance.GeneratedRoomDict.Count;     //表示当前有多少房间

        if (roomNum <= m_StoneNum)      //房间数量不足以生成所有祷告石时
        {

        }

        else
        {
            int randomNum = UnityEngine.Random.Range(0, roomNum);       //随机房间索引
        }
    }
}