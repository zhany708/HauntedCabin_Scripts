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

    bool m_NeedGenerateStone = false;   //判断是否需要生成祷告石
    public int m_NeededStoneNum = 2;    //需要生成的祷告石的数量
    int m_GeneratedStoneNum = 0;        //表示当前生成了多少祷告石



    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //玩家死亡时停止协程
        RoomManager.Instance.OnRoomGenerated += GenerateStoneAtSingleRoom;      //新生成房间时，调用此函数
    }

    private void OnDisable()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnHealthZero -= DestroyCoroutine;
        }

        RoomManager.Instance.OnRoomGenerated += GenerateStoneAtSingleRoom;
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



    //尝试生成所有的祷告石（整个剧本只调用一次）
    public void GenerateRitualStones()       //在随机房间生成祷告石
    {
        List<Vector2> tempRoomPos = new List<Vector2>();    //用于储存所有房间字典里的坐标

        //将字典里的所有坐标储存在列表中
        foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
        {
            tempRoomPos.Add(room);
        }

        tempRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage());   //移除触发进入二阶段的房间的坐标，防止玩家立刻获得祷告石


        //判断房间数量是否足够生成所有祷告石
        if (tempRoomPos.Count <= m_NeededStoneNum)      //房间数量不足以生成所有祷告石时
        {
            //需要做的：在后续房间生成后强行生成祷告石
            GenerateSeveralStones(tempRoomPos.Count, tempRoomPos);      //能生成多少祷告石，就生成多少

            m_NeedGenerateStone = true;
        }

        else
        {
            GenerateSeveralStones(m_NeededStoneNum, tempRoomPos);       //生成所有祷告石
        }
    }

    private void GenerateSeveralStones(int generatedNum, List<Vector2> roomPosList)     //生成参数中的数量的祷告石
    {
        //将所需的祷告石全部生成出来
        for (int i = 0; i < generatedNum; i++)
        {
            int randomNum = UnityEngine.Random.Range(0, roomPosList.Count); //随机房间索引
            Vector2 selectedRoomPos = roomPosList[randomNum];               //获取随机选择的房间的坐标
            roomPosList.RemoveAt(randomNum);                                //移除已选择的房间以防止重复

            //在选中的房间生成祷告石
            EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[selectedRoomPos].transform, selectedRoomPos);

            m_GeneratedStoneNum++;      //增加祷告石计数
        }
    }

    //在单独的房间生成祷告石
    private void GenerateStoneAtSingleRoom(Vector2 roomPos)
    {
        if (m_NeedGenerateStone)
        {
            //在参数中的房间生成祷告石
            EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[roomPos].transform, roomPos);

            m_GeneratedStoneNum++;      //增加祷告石计数
        }        

        if (m_GeneratedStoneNum >= m_NeededStoneNum)        //判断是否已经生成了足够的祷告石
        {
            m_NeedGenerateStone = false;
        }
    }
}