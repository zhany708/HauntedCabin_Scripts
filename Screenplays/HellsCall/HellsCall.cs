using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public class HellsCall : BaseScreenplay<HellsCall>
{
    public event Action OnRitualFinished;       //接收方为TaskPanel


    public GameObject RitualStone;      //祷告石物体

    
    public Stats PlayerStats      //Lazy load
    {
        get
        {
            if (m_PlayerStats == null)
            {
                m_PlayerStats = FindAnyObjectByType<Stats>();
            }
            return m_PlayerStats;
        }
    }
    private Stats m_PlayerStats;

    public const string RitualRoomName = "RitualRoom";      //加进Key里的仪式房的名字


    public DoorController RitualRoomDoorController { get; private set; }    //仪式房的DoorController脚本

    //用于储存所有仍然存在的祷告石的坐标和物体（如果玩家拿走了则从列表中移除）
    public Dictionary<Vector2, GameObject> AllStonePosDict { get; private set; } = new Dictionary<Vector2, GameObject>();


    Coroutine m_HealthDrainCoroutine;       //玩家持续掉血的协程
    Coroutine m_FireEffectCoroutine;        //火焰滤镜的协程

    List<Vector2> m_AllRoomPos = new List<Vector2>();     //用于储存所有房间字典里的坐标
    


    bool m_NeedGenerateStone = false;   //判断是否需要生成祷告石
    bool m_CanStartRitual = false;      //判断是否可以开始仪式


    [SerializeField] int m_NeededStoneNum = 3;           //需要生成的祷告石的数量（也是玩家需要达成的仪式数量）
    int m_GeneratedStoneNum = 0;        //表示当前生成了多少祷告石
    int m_MaxAllowedRoomNum = 0;        //一楼可以生成的最大房间数
    int m_FinishedRitualCount = 0;      //玩家完成的仪式数量

    [SerializeField] float m_HealthDrainDamageAmount = 1f;  //在掉血函数中每次掉血的数量
    [SerializeField] float m_HealthDrainInterval = 12f;     //在掉血函数中每（60/此变量）秒掉一次血




    #region Unity内部函数
    private void OnEnable()
    {
        PlayerStats.OnHealthZero += DestroyCoroutine;                           //玩家死亡时停止持续掉血的协程
        RoomManager.Instance.OnRoomGenerated += GenerateStoneAtSingleRoom;      //新生成房间时，检查是否生成祷告石
    }

    private void OnDisable()
    {
        if (PlayerStats != null)
        {
            PlayerStats.OnHealthZero -= DestroyCoroutine;
        }

        RoomManager.Instance.OnRoomGenerated -= GenerateStoneAtSingleRoom;
        RoomManager.Instance.OnRoomGenerated -= CheckRitualRoomAfterGeneratingRoom;
        RootRoomController.OnPlayerFirstTimeEnterRoom -= CheckRitualRoomAfterPlayerEnteringRoom;
    }

    private void OnDestroy()
    {
        //将仪式房的名字从列表中移除，因为ScriptObject的记录不会随着游戏结束而消失
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(RitualRoomName);
        }

        DestroyCoroutine();     //停止持续掉血的协程
    }
    #endregion


    #region 剧本模板函数
    public override async Task StartScreenplay()
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCallBackground);   //打开剧本背景界面

        //先将当前所有房间的坐标储存进一个列表
        AddAllRoomPosIntoList();    
        //移除触发进入二阶段的房间的坐标，防止玩家立刻获得祷告石
        m_AllRoomPos.Remove(EventManager.Instance.GetRoomPosWhereEnterSecondStage());

        GenerateRitualStones();     //生成祷告石                                                                                                                                                                                                                                                                                                                                                                                      666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666666
        GenerateRitualRoom();       //生成仪式房
    }

    public override void ResetGame()
    {
        if (AllStonePosDict.Count != 0)
        {
            //删除所有仍然存在的祷告石物体
            foreach (var stone in AllStonePosDict)
            {
                Destroy(stone.Value);
            }
        }
    }

    public override async Task Victory()
    {
        DestroyCoroutine();     //停止玩家掉血和火焰滤镜的协程

        //打开入口大堂的大门
        MainDoorController.Instance.SetDoOpenMainDoor(true);       //设置布尔，以便玩家再次进入入口大堂后，大宅的大门会开启

        //提前加载好剧本胜利界面
        await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.HellsCall_GameWinningPanel);
    }

    public override async Task Lose()
    {
        EnvironmentManager.Instance.SetIsGameLost(true);      //设置布尔，表示游戏失败，同时防止执行正常仪式结束的逻辑
        EnemyPool.Instance.KillAllEnemy_DefenseWar();         ////立刻消灭所有敌人（防止敌人攻击玩家）


        Altar altar = FindAnyObjectByType<Altar>();
        if (altar == null)
        {
            Debug.LogError("Cannot get the Altar component in the : " + name);
            return;
        }

        altar.Combat.gameObject.SetActive(false);            //禁用祷告石的战斗组件，防止鞭尸


        //打开剧本失败界面
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.HellsCall_GameLostPanel);
    }
    #endregion


    #region 玩家持续掉血相关
    public void StartHealthDrain()      //开始持续掉血（将火焰滤镜与持续掉血联系起来，从而让它们可以同步）
    {
        if (PlayerStats != null)
        {
            //持续10000000秒，每次掉（参数二）点血，每（60/参数三）秒掉一次
            m_HealthDrainCoroutine = StartCoroutine(PlayerStats.HealthDrain(10000000f, m_HealthDrainDamageAmount, m_HealthDrainInterval));
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


    #region 仪式房相关
    private void GenerateRitualRoom()       //生成仪式房（整个地图只有一个）
    {
        RoomManager.Instance.GetMaxAllowedRoomNum(ref m_MaxAllowedRoomNum);       //一楼可以生成的最大房间数（当前为35）

        //当没有新的房间可以生成时
        if (RoomManager.Instance.GeneratedRoomDict.Count >= m_MaxAllowedRoomNum)
        {
            Vector2 randomRoomPos = GenerateSuitableRandomRoomPos();     //随机选择的房间的坐标

            StartCoroutine(DelayedRegenerateRoom(randomRoomPos) );       //删除参数中坐标处的房间，随后在该坐标生成仪式房
        }

        else
        {
            //将检查仪式房的函数绑定到房间管理器中的事件
            RoomManager.Instance.OnRoomGenerated += CheckRitualRoomAfterGeneratingRoom;        //新生成房间时，检查是否需要生成仪式房
            RootRoomController.OnPlayerFirstTimeEnterRoom += CheckRitualRoomAfterPlayerEnteringRoom;    //玩家进入房间后，检查是否需要生成仪式房

            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Add(RitualRoomName);       //将仪式房的名字加进列表，以便后续可以生成
        }
    }

    private void CheckRitualRoomAfterGeneratingRoom(Vector2 roomPos)        //每当一个房间新生成后，检查仪式房是否已经生成
    {
        //先检查仪式房是否已经生成
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(RitualRoomName) )
        {
            //当生成了最后一个房间后（因为运行顺序的原因，这里要大于等于1，而不是0。否则会出现生成完所有房间后依然没有仪式房的情况）
            if (m_MaxAllowedRoomNum - RoomManager.Instance.GeneratedRoomDict.Count <= 1)
            {
                StartCoroutine(DelayedRegenerateRoom(roomPos) );           //删除参数中坐标处的房间，随后在该坐标生成仪式房
            }
        }     
    }

    private void CheckRitualRoomAfterPlayerEnteringRoom(Vector2 roomPos)    //每当玩家进入房间后，检查仪式房是否已经生成
    {
        //先检查仪式房是否已经生成
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(RitualRoomName))
        {
            //当字典中所有房间玩家都已经进过时
            if (CheckIfPlayerEnterAllRoom())
            {
                AddAllRoomPosIntoList();    //先将当前所有房间的坐标储存进列表

                //移除玩家当前所在的房间的坐标（防止玩家立刻进入仪式房）
                m_AllRoomPos.Remove(roomPos);

                //移除祷告石所在的房间
                foreach (var stone in AllStonePosDict)
                {
                    if (m_AllRoomPos.Contains(stone.Key))
                    {
                        m_AllRoomPos.Remove(stone.Key);
                    }
                }



                Vector2 randomRoomPos = GenerateSuitableRandomRoomPos();     //随机选择的房间的坐标

                StartCoroutine(DelayedRegenerateRoom(randomRoomPos));           //删除参数中坐标处的房间，随后在该坐标生成仪式房
            }
        }            
    }

    private bool CheckIfPlayerEnterAllRoom()           //检查玩家是否进入过所有房间了
    {
        foreach (var room in RoomManager.Instance.GeneratedRoomDict.Values)
        {
            //获取房间的控制器脚本
            RootRoomController currentRoomController = room.GetComponent<RootRoomController>();
            if (currentRoomController == null)
            {
                Debug.LogError("Cannot get the RootRoomController component in the " + room.name);
            }      

            if (currentRoomController.GetFirstTimeEnterRoom() )       //只要有一个房间还没进过，就返回false
            {
                return false;
            }
        }

        //当所有房间都检查过且没有返回时（也就是说所有房间都已经进过了），返回true
        return true;
    }

    private IEnumerator DelayedRegenerateRoom(Vector2 roomPos)         //用于等待一帧后删除已有的房间，随后生成仪式房
    {
        yield return new WaitForEndOfFrame();       //等待一帧的结束，以便所有其余的所需内容都已初始化完成

        //删除最后的房间，将仪式房生成在这里
        if (RoomManager.Instance.GeneratedRoomDict.TryGetValue(roomPos, out GameObject deletedRoom))   //尝试从字典中获取对应的房间
        {
            Destroy(deletedRoom);       //删除最后的房间

            RoomManager.Instance.GenerateRoomAtThisPos(roomPos, RitualRoomName);    //将仪式房生成在这里
        }

        else
        {
            Debug.LogError("A room has generated here, but cannot get the corresponding gameObject: " + roomPos);
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
            int randomNum = UnityEngine.Random.Range(0, m_AllRoomPos.Count);   //随机房间索引
            selectedRoomPos = m_AllRoomPos[randomNum];     //获取随机选择的房间的坐标

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
        //判断房间数量是否足够生成所有祷告石
        if (m_AllRoomPos.Count <= m_NeededStoneNum)          //房间数量不足以生成所有祷告石时
        {
            GenerateSeveralStones(m_AllRoomPos.Count);       //能生成多少祷告石，就生成多少

            m_NeedGenerateStone = true;                      //在后续房间生成后强行生成祷告石
        }

        else
        {
            GenerateSeveralStones(m_NeededStoneNum);         //生成所有祷告石
        }
    }

    private void GenerateSeveralStones(int generatedNum)     //生成参数中的数量的祷告石
    {
        List<int> alreadyGeneratedRoomCount = new List<int>();       //创建一个临时列表，储存所有已经生成的房间的索引（用于防止重复生成祷告石）
        int randomNum = 0;

        //将所需的祷告石全部生成出来
        for (int i = 0; i < generatedNum; i++)
        {
            bool isDone = false;

            //该while循环用于生成随机房间索引（由于调用这个函数前就已经判断过当前的房间数量，因此无需担心房间不够的情况）
            while (!isDone)     
            {
                randomNum = UnityEngine.Random.Range(0, m_AllRoomPos.Count);           //随机房间索引

                if (alreadyGeneratedRoomCount.Contains(randomNum) )     //判断是否随机到了之前生成过的房间索引
                {
                    isDone = false;
                }
                else
                {
                    isDone = true;
                }
            }

            Vector2 selectedRoomPos = m_AllRoomPos[randomNum];    //获取随机选择的房间的坐标
            alreadyGeneratedRoomCount.Add(randomNum);             //将随机到的索引加进临时列表

            //在选中的房间生成祷告石
            EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[selectedRoomPos].transform, selectedRoomPos);

            m_GeneratedStoneNum++;                                //增加祷告石计数
        }
    }

    //在单独的房间生成祷告石
    private void GenerateStoneAtSingleRoom(Vector2 roomPos)
    {
        if (m_NeedGenerateStone)
        {
            //在参数中的房间生成祷告石
            EnvironmentManager.Instance.GenerateObjectWithParent(RitualStone, RoomManager.Instance.GeneratedRoomDict[roomPos].transform, roomPos);

            m_GeneratedStoneNum++;                          //增加祷告石计数
        }        

        if (m_GeneratedStoneNum >= m_NeededStoneNum)        //判断是否已经生成了足够的祷告石
        {
            m_NeedGenerateStone = false;
        }
    }
    #endregion


    #region 其余函数
    //将房间字典里的所有坐标储存在列表中
    private void AddAllRoomPosIntoList()
    {
        m_AllRoomPos.Clear();      //开始复制坐标前先清空列表

        
        foreach (var room in RoomManager.Instance.GeneratedRoomDict.Keys)
        {
            if (!m_AllRoomPos.Contains(room) )     //只有当列表中没有字典里的坐标时，才储存坐标（防止重复储存）
            {
                m_AllRoomPos.Add(room);
            }
        }      
    }


    //仪式完成后调用此函数
    public async void IncrementRitualCount()
    {
        m_FinishedRitualCount++;

        OnRitualFinished?.Invoke();        //调用回调函数

        if (m_FinishedRitualCount >= m_NeededStoneNum)      //当玩家完成所有仪式后
        {
            await Victory();
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

    public int GetNeededStoneNum()
    {
        return m_NeededStoneNum;
    }

    public int GetFinishedRitualCount()
    {
        return m_FinishedRitualCount;
    }
    #endregion
}