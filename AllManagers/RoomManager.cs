using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class RoomManager : ManagerTemplate<RoomManager>
{
    public delegate void RoomGeneratedHandler(Vector2 roomPosition);        //delegate是用于限制引用事件的函数的参数（这里是必须有Vector2参数）
    public event RoomGeneratedHandler OnRoomGenerated;                      //使用上面的限制的事件。接收方为HellsCall脚本


    public SO_RoomKeys RoomKeys;            //生成房间需要的所有房间名
    public LayerMask RoomLayerMask;         //房间的图层

    //限制墙壁的大小，可根据难度的不同调整
    public float MaximumXPos = 35f;
    public float MaximumYPos = 35f;


    //以下是所有房间里的门的统一名字
    public const string LeftDoorName = "LeftDoor";
    public const string RightDoorName = "RightDoor";
    public const string DownDoorName = "DownDoor";
    public const string UpDoorName = "UpDoor";

    public const float RoomLength = 17f;        //房间的长
    public const float RoomWidth = 10.7f;       //房间的宽

       

    //储存加载过的房间的坐标和物体，用于检查是否有连接的门
    public Dictionary<Vector2, GameObject> GeneratedRoomDict { get; private set; } = new Dictionary<Vector2, GameObject>();

    //用于储存所有不可更改的房间坐标（比如初始房间等）              后面要做的：添加其余的一楼初始板块
    public List<Vector2> ImportantRoomPos { get; private set; } = new List<Vector2>();


    
    Transform m_FirstFloorRooms;       //所有生成的一楼房间的父物体（为了整洁美观）


    //运用Physics2D检查重复坐标时需要的X和Y的值
    const float m_PhysicsCheckingXPos = 15f;
    const float m_PhysicsCheckingYPos = 10f;

    int m_RandomGeneratedNum = -1;      //随机生成的数（用于新的房间生成的索引）










    #region Unity内部函数循环
    protected override void Awake()
    {
        base.Awake();

        if (RoomKeys == null || MaximumXPos <= 0 || MaximumYPos <= 0)
        {
            Debug.LogError("Some components are not correctly assigned in the RoomManager");
            return;          
        }

        //赋值房间跟物体给脚本中的变量
        SetupRootGameObject(ref m_FirstFloorRooms, "FirstFloorRooms");

        //将入口大堂的坐标加进重要房间坐标列表，以在重置游戏时不被删除
        //ImportantRoomPos.Add(EntranceHall.Instance.transform.position);
    }

    private void Start()
    {
        //自动设置层级
        //RoomLayerMask = LayerMask.GetMask("OnlyTriggerPlayerAndEnemy");

        if (RoomKeys.FirstFloorRoomKeys.Contains("RitualRoom"))     //开始游戏前检查是否有只在剧本中才会生成的房间名，防止出现错误
        {
            RoomKeys.FirstFloorRoomKeys.Remove("RitualRoom");
        }
    }
    #endregion


    #region 房间生成函数
    //生成房间
    public void GenerateRoomAtAround(Transform currentRoomTransform, RoomType currentRoomType)
    {
        DoorFlags currentDoorFlags = currentRoomType.GetDoorFlags();

        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Left) != 0, new Vector2(-RoomLength, 0), RightDoorName);
        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Right) != 0, new Vector2(RoomLength, 0), LeftDoorName);
        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Up) != 0, new Vector2(0, RoomWidth), DownDoorName);
        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Down) != 0, new Vector2(0, -RoomWidth), UpDoorName);
    }

    private void GenerateRoomInDirection(Transform currentRoomTransform, bool hasDoor, Vector2 offset, string neededDoorName)
    {

        if (hasDoor)
        {
            Vector2 newRoomPos = (Vector2)currentRoomTransform.position + offset;

            //如果新生成的房间超出限制墙壁，则返回
            if (CheckIfBreakMaximumPos(currentRoomTransform, newRoomPos)) return;


            if (CanGenerateRoomAtPosition(currentRoomTransform.position, offset))
            {
                //如果目标位置为空，则生成房间
                GenerateRandomRoom(currentRoomTransform, newRoomPos, neededDoorName);
            }

            else
            {
                //当目标生成位置已经有房间时，检查那个房间是否有连接当前房间的门，没有的话就放障碍物阻止玩家通过
                CheckRequiredDoorAtOverlapPosition(currentRoomTransform, newRoomPos, neededDoorName);

                //Debug.Log("Cannot generate room at this repeated position: " + newRoomPos);
            }
        }
    }

    private async void GenerateRandomRoom(Transform currentRoomTransform, Vector2 newRoomPos, string neededDoorName)
    {
        GameObject newRoom = null;
        bool isRoomPlaced = false;
        int attemptCount = 0;           //表示尝试了多少次
        const int maxAttempts = 50;     //最大尝试次数

        while (!isRoomPlaced && attemptCount <= maxAttempts)     //生成房间次数大于50次后强制返回，防止出现无限循环
        {
            attemptCount++;

            m_RandomGeneratedNum = UnityEngine.Random.Range(0, RoomKeys.FirstFloorRoomKeys.Count);       //随机生成房间的索引   ToDo：需要决定生成哪一层的房间


            //确认随机索引后尝试异步加载房间
            try
            {
                GameObject loadedRoom = await LoadPrefabAsync(RoomKeys.FirstFloorRoomKeys[m_RandomGeneratedNum]);       //异步加载房间
                if (loadedRoom != null)
                {
                    //如果因为场景加载等原因导致房间跟物体被删除过，就重新获取
                    if (m_FirstFloorRooms == null)
                    {
                        //赋值房间跟物体给脚本
                        SetupRootGameObject(ref m_FirstFloorRooms, "FirstFloorRooms");
                    }

                    //加载成功后，将房间生成出来
                    newRoom = Instantiate(loadedRoom, newRoomPos, Quaternion.identity, m_FirstFloorRooms);
                }

                else
                {
                    Debug.LogError("Failed to load room: " + RoomKeys.FirstFloorRoomKeys[m_RandomGeneratedNum]);
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("Error loading room: " + ex.Message);
            }


            RoomType newRoomType = newRoom.GetComponent<RoomType>();

            //检查是否有需要的门，如果没有则摧毁房间
            if (HasRequiredDoor(newRoomType, neededDoorName))
            {
                //Debug.Log("Generated a room at: " + newRoomPos);

                isRoomPlaced = true;

                //设置布尔表示该房间已经生成过房间了
                RootRoomController currentRoomController = currentRoomTransform.GetComponent<RootRoomController>();
                if (currentRoomController != null)
                {
                    currentRoomController.SetHasGenerateRoom(true);
                }

                else
                {
                    Debug.LogError("Cannot get the RootRoomController component in the: " + currentRoomTransform.name);
                }

                //触发事件
                OnRoomGenerated?.Invoke(newRoomPos);    //将新房间的坐标连接到事件
            }

            else
            {
                Destroy(newRoom);
                ReleasePrefab(newRoom.name);
            }
        }

        if (!isRoomPlaced)        //如果超过最大尝试次数后依然没有合适的房间
        {
            Debug.Log("Failed to generate a suitable room after " + maxAttempts + " attempts for the: " + currentRoomTransform.name);

            GenerateRoomAtThisPos(newRoomPos, RoomKeys.GenericRoomKey);     //强行生成通用房间
        }
    }

    public async void GenerateRoomAtThisPos(Vector2 newRoomPos, string roomKey)     //将房间生成在参数中的坐标处，不检查门是否连接
    {
        //尝试异步加载房间
        try
        {
            GameObject loadedRoom = await LoadPrefabAsync(roomKey);       //异步加载房间
            if (loadedRoom != null)
            {
                //如果因为场景加载等原因导致房间跟物体被删除过，就重新获取
                if (m_FirstFloorRooms == null)
                {
                    //赋值房间跟物体给脚本
                    SetupRootGameObject(ref m_FirstFloorRooms, "FirstFloorRooms");
                }

                //加载成功后，将房间生成出来
                GameObject newRoom = Instantiate(loadedRoom, newRoomPos, Quaternion.identity, m_FirstFloorRooms);
            }

            else
            {
                Debug.LogError("Failed to load room: " + roomKey);
                return;
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error loading room: " + ex.Message);
            return;
        }
    }
    #endregion


    #region 检查函数

    //检查当前房间是否连接周围的房间（玩家每进入一个房间时都需要调用此函数）
    public void CheckIfConnectSurroundingRooms(Transform thisRoomTransform)       
    {
        RoomType currentRoomType = thisRoomTransform.GetComponent<RoomType>();   //获取当前房间物体上挂载的房间类型脚本
        if (currentRoomType == null)
        {
            Debug.LogError("Cannot get the RoomType component in the " + currentRoomType.gameObject.name);
            return;
        }    


        if (!currentRoomType.HasCheckFlag(CheckFlags.Left))     //如果当前房间没有检查过左边
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(-RoomLength, 0), RightDoorName);
        }

        if (!currentRoomType.HasCheckFlag(CheckFlags.Right))    //如果当前房间没有检查过右边
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(RoomLength, 0), LeftDoorName);
        }

        if (!currentRoomType.HasCheckFlag(CheckFlags.Up))       //如果当前房间没有检查过上面
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(0, RoomWidth), DownDoorName);
        }

        if (!currentRoomType.HasCheckFlag(CheckFlags.Down))     //如果当前房间没有检查过下面
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(0, -RoomWidth), UpDoorName);
        }       
    }
    

    //检查参数中坐标处是否已经有房间，有的话则检查该房间是否有连接当前房间的门
    private void CheckRequiredDoorAtOverlapPosition(Transform currentRoomTransform, Vector2 checkPos, string neededDoorName)
    {
        if (GeneratedRoomDict.ContainsKey(checkPos))
        {
            //Debug.Log("A room has already generated here: " + checkPos);

            //检查能否获取字典对应的坐标的房间物体
            if (GeneratedRoomDict.TryGetValue(checkPos, out GameObject repeatedRoom))
            {
                RoomType currentRoomType = currentRoomTransform.GetComponent<RoomType>();   //获取当前房间物体上挂载的房间类型脚本  
                RoomType targetRoomType = repeatedRoom.GetComponent<RoomType>();            //获取目标位置处房间物体上挂载的房间类型脚本       
                string closedDoorName = null;                      //表示要关闭的房间门的名字

                //根据需要的房间名设置当前房间的RoomType脚本里检查旗帜里对应的布尔值，以及木桶的坐标
                switch (neededDoorName)
                {
                    case LeftDoorName:
                        currentRoomType.SetCheckFlag(CheckFlags.Right);   //表示检查过是否连接右边的房间了

                        closedDoorName = RightDoorName;
                        break;

                    case RightDoorName:
                        currentRoomType.SetCheckFlag(CheckFlags.Left);    //表示检查过是否连接左边的房间了

                        closedDoorName = LeftDoorName;
                        break;

                    case UpDoorName:
                        currentRoomType.SetCheckFlag(CheckFlags.Down);    //表示检查过是否连接下面的房间了

                        closedDoorName = DownDoorName;
                        break;

                    case DownDoorName:
                        currentRoomType.SetCheckFlag(CheckFlags.Up);      //表示检查过是否连接上面的房间了

                        closedDoorName = UpDoorName;
                        break;

                    default:
                        Debug.Log("There is a room generated at this position: " + checkPos + ", but none of its door match the neededDoorName: " + neededDoorName);
                        break;
                };



                //检查重复坐标的房间是否有连接此房间的门，如果没有则强行关闭该门
                if (!HasRequiredDoor(targetRoomType, neededDoorName))
                {
                    HandleUnconnectedDoorsCondition(currentRoomTransform, closedDoorName);
                }
            }

            else
            {
                Debug.LogError("A room has generated here, but cannot get the corresponding gameObject: " + checkPos);
                return;
            }
        }
    }



    //运用物理函数，检查要生成的坐标是否已经有房间了
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 roomSize)
    {
        //第一个参数为中心点，第二个参数为检查的正方形大小，第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, roomSize, 0f, RoomLayerMask);
        return overlapCheck == null;
    }


    //检查是否可以在目标位置生成房间
    private bool CanGenerateRoomAtPosition(Vector2 currentRoomPos, Vector2 offset)
    {
        Vector2 newRoomPos = currentRoomPos + offset;

        //以目标点（上面的newRoomPos）为中心点开始的长方形大小
        Vector2 roomSize = new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos);

        //检查这个长方形大小里是否有房间图层
        return IsPositionEmpty(newRoomPos, roomSize);
    }


    //检查参数中房间类型脚本，从而确定房间是否有需要的门
    private bool HasRequiredDoor(RoomType roomType, string neededDoorName)
    {
        DoorFlags newDoorFlags = roomType.GetDoorFlags();

        return neededDoorName switch        //根据需要的房间名返回对应的布尔值
        {
            LeftDoorName => (newDoorFlags & DoorFlags.Left) != 0,
            RightDoorName => (newDoorFlags & DoorFlags.Right) != 0,
            UpDoorName => (newDoorFlags & DoorFlags.Up) != 0,
            DownDoorName => (newDoorFlags & DoorFlags.Down) != 0,
            _ => false,     //相当于default case，如果上面四个都没有实施，则实施这一行
        };
    }


    //该函数在房间超出墙壁时会永久关闭对应的门，而不是生成木桶阻碍玩家前进
    private bool CheckIfBreakMaximumPos(Transform currentRoomTransform, Vector2 newRoomPos)
    {
        //如果要生成的房间的坐标超出了限制墙壁时，则关闭该房间对应的门
        if (Mathf.Abs(newRoomPos.x) >= MaximumXPos || Mathf.Abs(newRoomPos.y) >= MaximumYPos)
        {
            string closedDoorName = null;       //永久关闭的门的名字


            if (Mathf.Abs(newRoomPos.x) >= MaximumXPos)
            {           
                //根据生成的坐标判断哪个门应该关闭
                closedDoorName = newRoomPos.x <= 0 ? LeftDoorName : RightDoorName;
            }

            else if (Mathf.Abs(newRoomPos.y) >= MaximumYPos)
            {
                closedDoorName = newRoomPos.y <= 0 ? DownDoorName : UpDoorName;               
            }


            HandleUnconnectedDoorsCondition(currentRoomTransform, closedDoorName);
            return true;
        } 

        else
        {
            return false;
        }
    }

    /*
    public bool CheckIfAllRoomHasGenerated()           //检查字典中是否所有房间都已经生成过了
    {
        foreach (var room in GeneratedRoomDict.Values)
        {
            //获取房间的控制器脚本
            RootRoomController currentRoomController = room.GetComponent<RootRoomController>();
            if (currentRoomController == null)
            {
                Debug.LogError("Cannot get the RootRoomController component in the " + room.name);
            }

            if (!currentRoomController.GetHasGenerateRoom())       //只要有一个房间还没生成过，就返回false
            {
                return false;
            }
        }

        //当所有房间都检查过且没有返回时（也就是说所有房间都已经生成过了），返回true
        return true;
    }
    */
    #endregion


    #region 其余房间函数（规格，重置游戏等）
    //当遇到房间门不相连时调用的函数
    private void HandleUnconnectedDoorsCondition(Transform currentRoomTransform, string closedDoorName)
    {
        //获取当前房间的控制器脚本
        RootRoomController currentRoomController = currentRoomTransform.GetComponent<RootRoomController>();
        if (currentRoomController == null)
        {
            Debug.LogError("Cannot get the RootRoomController component in the " + currentRoomTransform.gameObject.name);
            return;
        }


        //将需要关闭的门加进列表
        currentRoomController.DoorControllerInsideThisRoom.AlwaysClosedDoorNames.Add(closedDoorName);
        //激活关闭的门对应的小地图门框  
        currentRoomController.MiniMapControllerInsideThisRoom.SetActiveBackupFrame(closedDoorName);

        //加进列表后立刻调用指定的函数，从而确保玩家进入该房间时永久关闭的门处于关闭状态
        currentRoomController.DoorControllerInsideThisRoom.CloseNecessaryDoors();
    }




    public void GetMaxAllowedRoomNum(ref int thisNum)       //获取可以生成的最大房间数
    {
        //一行可以生成的房间数量。FloorToInt函数用于将结果向下取整（无论小数部分有多大）
        int allowedRoomNumOnRow = Mathf.FloorToInt(MaximumXPos * 2 / RoomLength) + 1;

        //一列可以生成的房间数量
        int allowedRoomNumOnColumn = Mathf.FloorToInt(MaximumYPos * 2 / RoomWidth) + 1;

        thisNum = allowedRoomNumOnRow * allowedRoomNumOnColumn;       //一楼可以生成的最大房间数（当前为35）
    }

    public void PrintGeneratedRoomDict()        //用于DEBUG
    {
        //将字典里的所有房间的名字和对应的坐标打印出来
        foreach (var room in GeneratedRoomDict.Keys)
        {
            if (GeneratedRoomDict.TryGetValue(room, out GameObject tempEoomName))
            {
                Debug.Log(tempEoomName.name + "is at this position: " + room);
            }

            else
            {
                Debug.LogError("This Key is not found in the dictionary: " + room);
                return;
            }
        }
    }



    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //进入主菜单
        if (scene.name == SceneManagerScript.MainMenuSceneName)
        {
            //检查玩家是否第一次进入游戏
            if (!EnvironmentManager.Instance.IsFirstTimeEnterGame)
            {
                //删除所有普通房间，随后隐藏所有初始房间（只在非第一次进入游戏才会执行）
                ResetGame();
            }

            SetActiveAllRoom(false);
        }

        //进入一楼场景
        else if (scene.name == SceneManagerScript.FirstFloorSceneName)
        {
            //Debug.Log("SetActiveAllRoom function in RoomManager is called.");
            SetActiveAllRoom(true);      //激活所有房间
        }

        else
        {
            Debug.Log("We only have two scenes now, please check the parameters!");
        }
    }

    private void SetActiveAllRoom(bool isActive)
    {
        //在场景中激活/取消激活所有AllRoom下的房间
        foreach (Transform child in m_FirstFloorRooms)    
        {
            child.gameObject.SetActive(isActive);
        }
    }

    public void ResetGame()
    {
        foreach (Transform child in m_FirstFloorRooms)    //在场景中删除所有AllRoom下的房间（除了初始房间）
        {
            RootRoomController roomScript = child.GetComponent<RootRoomController>();
            if (roomScript == null)
            {
                Debug.LogError("Cannot get the RootRoomController script from the: " + child.name);
                return;
            }

            //如果是不可删除的房间（初始房间）
            if (ImportantRoomPos.Contains((Vector2)roomScript.gameObject.transform.position))
            {
                roomScript.ResetGame();             //调用重置游戏函数
            }   
            
            //只删除普通房间 
            else        
            {
                roomScript.ResetGame();             //先调用重置游戏函数
                Destroy(roomScript.gameObject);     //再删除物体
            }  
        }
    }
    #endregion
}