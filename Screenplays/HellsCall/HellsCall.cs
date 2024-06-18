using System.Collections.Generic;
using UnityEngine;



public class HellsCall : BaseScreenplay<HellsCall>
{
    public GameObject RitualStone;      //祷告石物体

    public DoorController RitualRoomDoorController { get; private set; }    //仪式房的DoorController脚本
    public PlayerStats PlayerStats      //Lazy load
    {
        get
        {
            if (m_PlayerStats == null)
            {
                m_PlayerStats = FindAnyObjectByType<PlayerStats>();
            }
            return m_PlayerStats;
        }
    }
    private PlayerStats m_PlayerStats;

    public string RitualRoomName { get; private set; } = "RitualRoom";      //加进Key里的仪式房的名字



    Coroutine m_HealthDrainCoroutine;       //玩家持续掉血的协程
    Coroutine m_FireEffectCoroutine;        //火焰滤镜的协程

    List<Vector2> m_TempRoomPos = new List<Vector2>();    //用于储存所有房间字典里的坐标


    bool m_NeedGenerateStone = false;   //判断是否需要生成祷告石
    bool m_CanStartRitual = false;      //判断是否可以开始仪式


    int m_NeededStoneNum = 2;           //需要生成的祷告石的数量（也是玩家需要达成的仪式数量）
    int m_GeneratedStoneNum = 0;        //表示当前生成了多少祷告石
    int m_MaxAllowedRoomNum = 0;        //一楼可以生成的最大房间数
    int m_FinishedRitualCount = 0;      //玩家完成的仪式数量





    #region Unity内部函数
    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;       //玩家死亡时停止持续掉血的协程
        RoomManager.Instance.OnRoomGenerated += GenerateStoneAtSingleRoom;      //新生成房间时，检查是否生成祷告石
    }

    private void OnDisable()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnHealthZero -= DestroyCoroutine;
        }

        RoomManager.Instance.OnRoomGenerated -= GenerateStoneAtSingleRoom;
        RoomManager.Instance.OnRoomGenerated -= CheckRitualRoomGeneration;
    }

    private void OnDestroy()
    {
        //将仪式房的名字从列表中移除，因为ScriptObject的记录不会随着游戏结束而消失
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(RitualRoomName);
        }      
    }
    #endregion


    public async override void StartScreenplay()
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallPanel);   //打开剧本背景界面

        AddAllRoomPosIntoList();
        GenerateRitualStones();     //生成祷告石                                                                                                                                                                                                                                                                                                                                                                                                 666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
        GenerateRitualRoom();       //生成仪式房
    }


    #region 玩家持续掉血相关
    public void StartHealthDrain()      //开始持续掉血（将火焰滤镜与持续掉血联系起来，从而让它们可以同步）
    {
        if (PlayerStats != null)
        {
            //持续10000000秒，每次掉1点血，每5秒掉一次
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(10000000f, 1f, 12f));
            m_FireEffectCoroutine = StartCoroutine(PostProcessController.Instance.StartFireEffect() );      //一直显示火焰滤镜
        }
    }

    private void DestroyCoroutine()     //停止协程
    {
        if (m_HealthDrainCoroutine != null)
        {
            //Debug.Log("Coroutine stopped!!");
            StopCoroutine(m_HealthDrainCoroutine);
        }

        if (m_FireEffectCoroutine != null)
        {
            PostProcessController.Instance.TurnOffVignette();       //关闭滤镜
            StopCoroutine(m_FireEffectCoroutine);
        }
    }
    #endregion


    private void AddAllRoomPosIntoList()
    {
        //Debug.Log("Now we have this number of rooms in the dict: " + RoomManager.Instance.GeneratedRoomDict.Count);
        //RoomManager.Instance.PrintGeneratedRoomDict();      //打印出字典里的所有房间

        m_TempRoomPos.Clear();      //开始复制坐标前先清空列表

        //将字典里的所有坐标储存在列表中
        foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
        {
            m_TempRoomPos.Add(room);
            /*
            if (!m_TempRoomPos.Contains(room) )     //只有当列表中没有字典里的坐标时，才储存坐标
            {
                m_TempRoomPos.Add(room);
            }  
            */
        }
        //移除触发进入二阶段的房间的坐标，防止玩家立刻获得祷告石
        m_TempRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage());
    }


    public void IncrementRitualCount()
    {
        m_FinishedRitualCount++;

        if (m_FinishedRitualCount >= m_NeededStoneNum)      //当玩家完成所有仪式后
        {
            DestroyCoroutine();     //停止玩家掉血和火焰滤镜的协程

            //打开入口大堂的大门
            MainDoorController.Instance.SetDoOpenMainDoor(true);       //设置布尔，以便玩家再次进入入口大堂后，大门会开启
        }
    }


    #region 仪式房相关
    private void GenerateRitualRoom()       //生成仪式房（整个地图只有一个）
    {
        RoomManager.Instance.GetMaxAllowedRoomNum(ref m_MaxAllowedRoomNum);       //一楼可以生成的最大房间数（当前为35）

        //当没有新的房间可以生成时
        if (RoomManager.Instance.GeneratedRoomDict.Count >= m_MaxAllowedRoomNum)
        {
            //AddAllRoomPosIntoList();

            Vector2 selectedRoomPos = GenerateSuitableRandomRoomPos();     //随机选择的房间的坐标

            //尝试从字典中获取对应的房间
            if (RoomManager.Instance.GeneratedRoomDict.TryGetValue(selectedRoomPos, out GameObject deletedRoom))   
            {
                Destroy(deletedRoom);       //删除随机坐标对应的房间

                RoomManager.Instance.GenerateRoomAtThisPos(selectedRoomPos, RitualRoomName);    //将仪式房生成在这里
            }

            else
            {
                Debug.LogError("A room has generated here, but cannot get the corresponding gameobject: " + selectedRoomPos);
            }
        }

        else
        {
            //将检查仪式房的函数绑定到房间管理器中的事件
            RoomManager.Instance.OnRoomGenerated += CheckRitualRoomGeneration;      //新生成房间时，检查是否需要生成仪式房

            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Add(RitualRoomName);       //将仪式房的名字加进列表，以便后续可以生成
        }
    }

    private void CheckRitualRoomGeneration(Vector2 roomPos)        //检查仪式房是否已经生成
    {
        //当生成了最后一个房间后，仪式房还没有出现时
        if (m_MaxAllowedRoomNum - RoomManager.Instance.GeneratedRoomDict.Count <= 0 && RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(RitualRoomName))
        {
            //删除最后的房间，将仪式房生成在这里
            if (RoomManager.Instance.GeneratedRoomDict.TryGetValue(roomPos, out GameObject deletedRoom))   //尝试从字典中获取对应的房间
            {
                Destroy(deletedRoom);       //删除最后的房间

                RoomManager.Instance.GenerateRoomAtThisPos(roomPos, RitualRoomName);    //将仪式房生成在这里
            }

            else
            {
                Debug.LogError("A room has generated here, but cannot get the corresponding gameobject: " + roomPos);
            }
        }
    }


    private Vector2 GenerateSuitableRandomRoomPos()    //生成合适的随机房间坐标（因为某些房间不可更改）
    {
        Vector2 selectedRoomPos = Vector2.zero;     //用于储存随机选择到的房间的坐标
        int attemptCount = 0;                       //表示尝试了多少次
        const int maxAttemptCount = 50;             //最大尝试次数

        //只要随机到不可更改的房间坐标，就重新获取随机索引
        while (RoomManager.Instance.ImportantRoomPos.Contains(selectedRoomPos) && attemptCount <= maxAttemptCount)
        {
            int randomNum = Random.Range(0, m_TempRoomPos.Count);   //随机房间索引
            selectedRoomPos = m_TempRoomPos[randomNum];     //获取随机选择的房间的坐标

            attemptCount++;     //增加尝试计数
        }

        if (attemptCount > maxAttemptCount)        //超过最大尝试次数（没有成功生成随机坐标）后报错
        {
            Debug.LogError("Failed to generate a suitable random room position after " + maxAttemptCount + " attempts!");
        }

        return selectedRoomPos;
    }
    #endregion


    #region 祷告石相关
    //尝试生成所有的祷告石（整个剧本只调用一次）
    public void GenerateRitualStones()       //在随机房间生成祷告石
    {
        //AddAllRoomPosIntoList();    //将字典里的所有坐标储存在列表中


        //判断房间数量是否足够生成所有祷告石
        if (m_TempRoomPos.Count <= m_NeededStoneNum)      //房间数量不足以生成所有祷告石时
        {
            GenerateSeveralStones(m_TempRoomPos.Count, m_TempRoomPos);      //能生成多少祷告石，就生成多少

            m_NeedGenerateStone = true;     //在后续房间生成后强行生成祷告石
        }

        else
        {
            GenerateSeveralStones(m_NeededStoneNum, m_TempRoomPos);       //生成所有祷告石
        }
    }

    private void GenerateSeveralStones(int generatedNum, List<Vector2> roomPosList)     //生成参数中的数量的祷告石
    {
        //将所需的祷告石全部生成出来
        for (int i = 0; i < generatedNum; i++)
        {
            int randomNum = Random.Range(0, roomPosList.Count);     //随机房间索引
            Vector2 selectedRoomPos = roomPosList[randomNum];       //获取随机选择的房间的坐标
            roomPosList.RemoveAt(randomNum);                        //移除已选择的房间以防止重复

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
    #endregion


    #region Setters
    public void SetRitualRoomDoorController(DoorController thisDoorController)      //仪式房生成后调用此函数
    {
        RitualRoomDoorController = thisDoorController;
    }

    public void SetCanStartRitual(bool isTrue)
    {
        //Debug.Log("Can player start the ritual?" + isTrue);

        m_CanStartRitual = isTrue;
    }
    #endregion


    #region Getters
    public bool GetCanStartRitual()
    {
        return m_CanStartRitual;
    }
    #endregion
}