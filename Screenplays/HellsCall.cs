using System.Collections.Generic;
using UnityEngine;



public class HellsCall : BaseScreenplay
{
    public GameObject RitualStone;      //祷告石物体


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
        if (PlayerStats != null)
        {
            PlayerStats.OnHealthZero -= DestroyCoroutine;
        }
    }

   



    public async override void StartScreenplay()
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //打开剧本背景界面
                                                                         
        GenerateRitualStones();     //生成祷告石
    }



   


    public void StartHealthDrain()      //开始持续掉血
    {
        if (PlayerStats != null)
        {
            //持续10秒，每次掉5点血，每5秒掉一次
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(10f, 5f, 12f));
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




    public void GenerateRitualStones()       //在随机房间生成祷告石
    {
        int roomNum = RoomManager.Instance.GeneratedRoomDict.Count;     //表示当前有多少房间

        if (roomNum <= m_StoneNum)      //房间数量不足以生成所有祷告石时
        {
            //需要做的：在后续房间生成后强行生成祷告石
        }

        else
        {
            List<Vector2> tempRoomPos = new List<Vector2>();    //用于储存所有房间字典里的坐标

            // 将字典里的所有坐标储存在列表中
            foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
            {
                tempRoomPos.Add(room);
            }

            tempRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage() );   //移除触发进入二阶段的房间的坐标，防止玩家立刻获得祷告石

            //将所需的祷告石全部生成出来
            for (int i = 0; i < m_StoneNum; i++)
            {
                int randomNum = UnityEngine.Random.Range(0, tempRoomPos.Count); //随机房间索引
                Vector2 selectedRoomPos = tempRoomPos[randomNum];               //获取随机选择的房间的坐标
                tempRoomPos.RemoveAt(randomNum);                                //移除已选择的房间以防止重复

                // 在选中的房间生成祷告石
                EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[selectedRoomPos].transform, selectedRoomPos);
            }
        }
    }
}