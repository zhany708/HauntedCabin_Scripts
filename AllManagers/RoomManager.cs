using System;
using System.Collections.Generic;
using UnityEngine;



public class RoomManager : ManagerTemplate<RoomManager>
{
    public delegate void RoomGeneratedHandler(Vector2 roomPosition);        //delegate是用于限制引用事件的函数的参数（这里是必须有Vector2参数）
    public event RoomGeneratedHandler OnRoomGenerated;                      //使用上面的限制的事件。接收方为HellsCall脚本


    public SO_RoomKeys RoomKeys;            //生成房间需要的所有房间名
    public LayerMask RoomLayerMask;         //房间的图层
    public GameObject BlockDoorBarrel;      //挡住玩家进入门的障碍物

    //限制墙壁的大小，可根据难度的不同调整
    public float MaximumXPos = 35f;
    public float MaximumYPos = 35f;

    public const float RoomLength = 17f;        //房间的长
    public const float RoomWidth = 10.7f;       //房间的宽

    //储存加载过的房间的坐标和物体，用于检查是否有连接的门
    public Dictionary<Vector2, GameObject> GeneratedRoomDict { get; private set; } = new Dictionary<Vector2, GameObject>();

    //用于储存所有不可更改的房间坐标（比如初始房间等），并将入口大堂加进列表    后面要做的：添加其余的一楼初始板块
    public List<Vector2> ImportantRoomPos { get; private set; } = new List<Vector2>() { Vector2.zero }; 


    
    Transform m_AllRooms;       //所有生成的房间的父物体（为了整洁美观）

    //用于生成挡住玩家进入门的障碍物的坐标
    Vector2 m_BlockUpDoor = new Vector2(0f, 3.7f);
    Vector2 m_BlockDownDoor = new Vector2(0f, -4.65f);
    Vector2 m_BlockLeftDoor = new Vector2(-7.55f, -0.7f);
    Vector2 m_BlockRightDoor = new Vector2(7.55f, -0.7f);


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

        //赋值房间跟物体给脚本
        SetupRootGameObject(ref m_AllRooms, "AllRooms");
    }

    private void Start()
    {
        //自动设置层级
        //RoomLayerMask = LayerMask.GetMask("OnlyTriggerPlayerAndEnemy");
    }


    private void OnEnable()
    {
        //该物体检查如果放在Awake函数中的话会报错
        if (BlockDoorBarrel == null)
        {
            Debug.LogError("BlockDoorBarrel is not assigned in the RoomManager.");
            return;
        }
    }
    #endregion


    #region 房间生成函数
    //生成房间
    public void GenerateRoomAround(Transform currentRoomTransform, RoomType currentRoomType)
    {
        DoorFlags currentDoorFlags = currentRoomType.GetDoorFlags();

        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Left) != 0, new Vector2(-RoomLength, 0), "RightDoor");
        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Right) != 0, new Vector2(RoomLength, 0), "LeftDoor");
        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Up) != 0, new Vector2(0, RoomWidth), "DownDoor");
        GenerateRoomInDirection( currentRoomTransform, (currentDoorFlags & DoorFlags.Down) != 0, new Vector2(0, -RoomWidth), "UpDoor");
    }

    private void GenerateRoomInDirection(Transform currentRoomTransform, bool hasDoor, Vector2 offset, string neededDoorName)
    {

        if (hasDoor)
        {
            Vector2 newRoomPos = (Vector2)currentRoomTransform.position + offset;

            //如果超出限制墙壁，则返回
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
                    if (m_AllRooms == null)
                    {
                        //赋值房间跟物体给脚本
                        SetupRootGameObject(ref m_AllRooms, "AllRooms");
                    }

                    //加载成功后，将房间生成出来
                    newRoom = Instantiate(loadedRoom, newRoomPos, Quaternion.identity, m_AllRooms);
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
                    currentRoomController.SetHasGeneratorRoom(true);
                }

                else
                {
                    Debug.LogError("Tried to set this room has generated rooms, but cannot get the RootRoomController: " + currentRoomTransform.name);
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

        if (!isRoomPlaced)        //如果超过最大尝试次数后依然没有合适的房间，则实施一些功能
        {
            Debug.Log("Failed to place a suitable room after " + maxAttempts + " attempts!");
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
                if (m_AllRooms == null)
                {
                    //赋值房间跟物体给脚本
                    SetupRootGameObject(ref m_AllRooms, "AllRooms");
                }

                //加载成功后，将房间生成出来
                GameObject newRoom = Instantiate(loadedRoom, newRoomPos, Quaternion.identity, m_AllRooms);
            }

            else
            {
                Debug.LogError("Failed to load room: " + roomKey);
            }
        }

        catch (Exception ex)
        {
            Debug.LogError("Error loading room: " + ex.Message);
        }
    }
    #endregion


    #region 检查函数

    //检查当前房间是否连接周围的房间（玩家每进入一个房间时都需要调用此函数）
    public void CheckIfConnectSurroundingRooms(Transform thisRoomTransform)       
    {
        RoomType currentRoomType = thisRoomTransform.GetComponent<RoomType>();   //获取当前房间物体上挂载的房间类型脚本      

        if (!currentRoomType.HasCheckFlag(CheckFlags.Left))     //如果当前房间没有检查过左边
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(-17f, 0), "RightDoor");
        }

        if (!currentRoomType.HasCheckFlag(CheckFlags.Right))    //如果当前房间没有检查过右边
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(17f, 0), "LeftDoor");
        }

        if (!currentRoomType.HasCheckFlag(CheckFlags.Up))       //如果当前房间没有检查过上面
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(0, 10.7f), "DownDoor");
        }

        if (!currentRoomType.HasCheckFlag(CheckFlags.Down))     //如果当前房间没有检查过下面
        {
            CheckRequiredDoorAtOverlapPosition(thisRoomTransform, (Vector2)thisRoomTransform.position + new Vector2(0, -10.7f), "UpDoor");
        }       
    }
    

    //检查参数中坐标对应的房间是否有连接当前房间的门
    private void CheckRequiredDoorAtOverlapPosition(Transform currentRoomTransform, Vector2 checkPos, string neededDoorName)
    {
        if (GeneratedRoomDict.ContainsKey(checkPos))
        {
            //Debug.Log("A room has already generated here: " + checkPos);

            GameObject repeatedRoom;

            //检查能否获取字典对应的坐标的房间物体
            if (GeneratedRoomDict.TryGetValue(checkPos, out repeatedRoom))
            {
                RoomType currentRoomType = currentRoomTransform.GetComponent<RoomType>();   //获取当前房间物体上挂载的房间类型脚本  
                RoomType targetRoomType = repeatedRoom.GetComponent<RoomType>();          //获取目标位置处房间物体上挂载的房间类型脚本       
                Vector2 blockObjectPos = Vector2.zero;      //表示生成的木桶的坐标


                //根据需要的房间名设置当前房间的RoomType脚本里检查旗帜里对应的布尔值，以及木桶的坐标
                switch (neededDoorName)
                {
                    case "LeftDoor":
                        currentRoomType.SetCheckFlag(CheckFlags.Right);   //表示检查过是否连接右边的房间了

                        blockObjectPos = m_BlockRightDoor;
                        break;

                    case "RightDoor":
                        currentRoomType.SetCheckFlag(CheckFlags.Left);    //表示检查过是否连接左边的房间了

                        blockObjectPos = m_BlockLeftDoor;
                        break;

                    case "UpDoor":
                        currentRoomType.SetCheckFlag(CheckFlags.Down);    //表示检查过是否连接下面的房间了

                        blockObjectPos = m_BlockDownDoor;
                        break;

                    case "DownDoor":
                        currentRoomType.SetCheckFlag(CheckFlags.Up);      //表示检查过是否连接上面的房间了

                        blockObjectPos = m_BlockUpDoor;
                        break;

                    default:
                        Debug.Log("There is a room want to generated at repeated position: " + checkPos + ", but none of its door match the neededDoorName: " + neededDoorName);
                        break;
                };



                //检查重复坐标的房间是否有连接此房间的门，如果没有则生成障碍物防止玩家穿过门
                if (!HasRequiredDoor(targetRoomType, neededDoorName))
                {
                    //在指定的门前生成障碍物
                    EnvironmentManager.Instance.GenerateObjectWithParent(BlockDoorBarrel, currentRoomTransform, (Vector2)currentRoomTransform.position + blockObjectPos);
                }
            }
        }
    }



    //运用物理函数，检查要生成的坐标是否已经有房间了
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 roomSize)
    {
        //第一个参数为中心点，第二个参数为正方形大小，第三个参数为角度，第四个参数为检测的目标层级
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
            "LeftDoor" => (newDoorFlags & DoorFlags.Left) != 0,
            "RightDoor" => (newDoorFlags & DoorFlags.Right) != 0,
            "UpDoor" => (newDoorFlags & DoorFlags.Up) != 0,
            "DownDoor" => (newDoorFlags & DoorFlags.Down) != 0,
            _ => false,     //相当于default case，如果上面四个都没有实施，则实施这一行
        };
    }


    //检查要生成的房间的坐标是否超出了限制墙壁
    private bool CheckIfBreakMaximumPos(Transform currentRoomTransform, Vector2 newRoomPos)
    {
        //如果要生成的房间的坐标超出了限制墙壁时，则在进入那个房间的门口处生成障碍物阻止玩家前进
        if (Mathf.Abs(newRoomPos.x) >= MaximumXPos)
        {
            //Debug.Log("Cannot generate new room at this position: " + newRoomPos);

            //生成木桶，用于阻止玩家穿过门
            Vector2 blockObjectPos = newRoomPos.x <= 0 ? m_BlockLeftDoor : m_BlockRightDoor;
            EnvironmentManager.Instance.GenerateObjectWithParent(BlockDoorBarrel, currentRoomTransform, (Vector2)currentRoomTransform.position + blockObjectPos);

            return true;
        }

        else if (Mathf.Abs(newRoomPos.y) >= MaximumYPos)
        {
            //生成木桶，用于阻止玩家穿过门
            Vector2 blockObjectPos = newRoomPos.y <= 0 ? m_BlockDownDoor : m_BlockUpDoor;
            EnvironmentManager.Instance.GenerateObjectWithParent(BlockDoorBarrel, currentRoomTransform, (Vector2)currentRoomTransform.position + blockObjectPos);

            return true;
        }

        return false;
    }
    #endregion


    #region 其余房间函数（规格，重置游戏等）
    public int MaxAllowedRoomNum()
    {
        //一行可以生成的房间数量。FloorToInt函数用于将结果向下取整（无论小数部分有多大）
        int allowedRoomNumOnRow = Mathf.FloorToInt(MaximumXPos * 2 / RoomLength) + 1;
        //一列可以生成的房间数量
        int allowedRoomNumOnColumn = Mathf.FloorToInt(MaximumYPos * 2 / RoomWidth) + 1;

        int maxAllowedRoomNum = allowedRoomNumOnRow * allowedRoomNumOnColumn;       //一楼可以生成的最大房间数（当前为35）
        return maxAllowedRoomNum;
    }

    public void ResetGame()
    {
        foreach (Transform child in m_AllRooms)    //在场景中删除所有AllRoom下的房间（除了初始房间）
        {
            RootRoomController childScript = child.GetComponent<RootRoomController>();
            if (childScript == null)
            {
                Debug.LogError("Cannot get the RootRoomController script from the: " + child.name);
                return;
            }

            //如果是不可删除的房间
            if (ImportantRoomPos.Contains((Vector2)childScript.gameobject.transform.position))
            {
                childScript.SetHasGeneratorRoom(false);     //重置布尔，以便玩家进入后可以重新生成房间
            }    
            else        //只删除非初始房间
            {
                Destroy(childScript.gameobject); 
            }  
        }
    }
    #endregion
}