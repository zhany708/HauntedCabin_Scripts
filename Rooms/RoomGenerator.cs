using System;
using System.Collections.Generic;
using UnityEngine;



public class RoomGenerator : ManagerTemplate<RoomGenerator>
{
    public SO_RoomKeys RoomKeys;
    public Transform FatherOfAllRooms;      //所有生成的房间的父物体，为了整洁美观
    public LayerMask roomLayerMask;         //房间的图层
    

    //储存加载过的房间的坐标和物体，用于检查是否有连接的门
    public Dictionary<Vector2, GameObject> GeneratedRoomDict = new Dictionary<Vector2, GameObject>();


    //限制墙壁的大小，可根据难度的不同调整
    public float MaximumXPos = 35f;
    public float MaximumYPos = 35f;



    //随机生成的数（用于新的房间生成的索引）
    int m_RandomGeneratedNum = -1;          

    //表示当前房间
    Transform m_CurrentRoomTransform;

    //运用Physics2D检查重复坐标时需要的X和Y的值
    const float m_PhysicsCheckingXPos = 15f;
    const float m_PhysicsCheckingYPos = 10f;


    //用于生成挡住玩家进入门的障碍物的坐标
    Vector2 m_BlockUpDoor = new Vector2(0f, 3.7f);
    Vector2 m_BlockDownDoor = new Vector2(0f, -4.65f);
    Vector2 m_BlockLeftDoor = new Vector2(-7.55f, -0.7f);
    Vector2 m_BlockRightDoor = new Vector2(7.55f, -0.7f);




    private void Start()
    {
        //自动设置层级
        //roomLayerMask = LayerMask.GetMask("OnlyTriggerPlayerAndEnemy");
    }


    //生成房间
    public void GenerateRoom(Transform currentRoom, RoomType currentRoomType)
    {
        m_CurrentRoomTransform = currentRoom;
        DoorFlags currentDoorFlags = currentRoomType.GetDoorFlags();

        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Left) != 0, new Vector2(-17f, 0), "RightDoor");
        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Right) != 0, new Vector2(17f, 0), "LeftDoor");
        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Up) != 0, new Vector2(0, 10.7f), "DownDoor");
        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Down) != 0, new Vector2(0, -10.7f), "UpDoor");
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
                GenerateSuitableRoom(newRoomPos, neededDoorName);
            }

            else
            {
                //当目标生成位置已经有房间时，检查那个房间是否有连接当前房间的门，没有的话就放障碍物阻止玩家通过
                CheckRequiredDoorAtOverlapPosition(newRoomPos, currentRoomTransform, neededDoorName);

                //Debug.Log("Cannot generate room at this repeated position: " + newRoomPos);
            }
        }
    }


    //运用物理函数检查要生成的坐标是否已经有房间了
    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 roomSize)
    {
        //第一个参数为中心点，第二个参数为正方形大小，第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, roomSize, 0f, roomLayerMask);
        return overlapCheck == null;
    }


    private bool CanGenerateRoomAtPosition(Vector2 currentRoomPos, Vector2 offset)
    {
        Vector2 newRoomPos = currentRoomPos + offset;

        //以目标点（上面的newRoomPos）为中心点开始的长方形大小
        Vector2 roomSize = new Vector2(m_PhysicsCheckingXPos, m_PhysicsCheckingYPos);

        //检查这个长方形大小里是否有房间图层
        return IsPositionEmpty(newRoomPos, roomSize);     
    }


    //检查参数中坐标对应的房间是否有连接当前房间的门
    private void CheckRequiredDoorAtOverlapPosition(Vector2 checkPos, Transform currentRoomTransform, string neededDoorName)
    {
        if (GeneratedRoomDict.ContainsKey(checkPos))
        {
            //Debug.Log("A room has already generated here: " + checkPos);

            GameObject repeatedRoom;

            //检查能否获取字典对应的坐标的房间物体
            if (GeneratedRoomDict.TryGetValue(checkPos, out repeatedRoom))
            {
                RoomType repeatedRoomType = repeatedRoom.GetComponent<RoomType>();

                //检查重复坐标的房间是否有连接此房间的门，如果没有则生成障碍物防止玩家穿过门
                if (!HasRequiredDoor(repeatedRoomType, neededDoorName))
                {
                    Vector2 blockObjectPos = Vector2.zero;

                    switch (neededDoorName)      //根据需要的房间名返回对应的布尔值
                    {
                        case "LeftDoor":
                            blockObjectPos = m_BlockRightDoor;
                            break;

                        case "RightDoor":
                            blockObjectPos = m_BlockLeftDoor;
                            break;

                        case "UpDoor":
                            blockObjectPos = m_BlockDownDoor;
                            break;

                        case "DownDoor":
                            blockObjectPos = m_BlockUpDoor;
                            break;

                        default:
                            Debug.Log("There is a room want to generated at repeated position: " + checkPos + ", but none of its door match the neededDoorName: " + neededDoorName);
                            break;
                    };

                    //在指定的门前生成障碍物
                    EnvironmentManager.Instance.GenerateBarrelToBlockDoor(currentRoomTransform, (Vector2)currentRoomTransform.position + blockObjectPos);
                }
            }
        }
    }

    


    private async void GenerateSuitableRoom(Vector2 newRoomPos, string neededDoorName)
    {
        GameObject newRoom = null;
        bool isRoomPlaced = false;
        int attemptCount = 0;
        const int maxAttempts = 50;     //最大尝试次数

        while (!isRoomPlaced && attemptCount < maxAttempts)     //生成房间次数大于50次后强制返回，防止出现无限循环
        {
            attemptCount++;

            m_RandomGeneratedNum = UnityEngine.Random.Range(0, RoomKeys.FirstFloorRoomKeys.Count);       //随机生成房间的索引   ToDo：需要决定生成哪一层的房间


            //确认随机索引后尝试异步加载房间
            try
            {
                GameObject loadedRoom = await LoadPrefabAsync(RoomKeys.FirstFloorRoomKeys[m_RandomGeneratedNum] );       //异步加载事件
                if (loadedRoom != null)
                {
                    //加载成功后，将房间生成出来
                    newRoom = Instantiate(loadedRoom, newRoomPos, Quaternion.identity, FatherOfAllRooms);
                }   

                else
                {
                    Debug.LogError("Failed to load room: " + RoomKeys.FirstFloorRoomKeys[m_RandomGeneratedNum] );
                }
            }

            catch (Exception ex)
            {
                Debug.LogError("Error loading room: " + ex.Message);
            }


            RoomType newRoomType = newRoom.GetComponent<RoomType>();

            //检查是否有需要的门，如果没有则摧毁房间
            if (HasRequiredDoor(newRoomType, neededDoorName) )
            {
                //Debug.Log("Generated a room at: " + newRoomPos);

                isRoomPlaced = true;

                //设置布尔表示该房间已经生成过房间了
                RootRoomController currentRoomController = m_CurrentRoomTransform.GetComponent<RootRoomController>();
                if (currentRoomController != null )
                {
                    currentRoomController.SetHasGeneratorRoom(true);
                }
                
                else
                {
                    Debug.LogError("Tried to set this room has generated rooms, but cannot get the RootRoomController: " + m_CurrentRoomTransform.name);
                }
            }

            else
            {
                Destroy(newRoom);
                ReleasePrefab(newRoom.name);
            }
        }

        if ( !isRoomPlaced )        //如果超过最大尝试次数后依然没有合适的房间，则实施一些功能
        {
            Debug.Log("Failed to place a suitable room after " + maxAttempts + " attempts!");
        }
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

            Vector2 blockObjectPos = newRoomPos.x <= 0 ? m_BlockLeftDoor : m_BlockRightDoor;
            EnvironmentManager.Instance.GenerateBarrelToBlockDoor(currentRoomTransform, (Vector2)currentRoomTransform.position + blockObjectPos);

            return true;
        }

        else if (Mathf.Abs(newRoomPos.y) >= MaximumYPos)
        {
            Vector2 blockObjectPos = newRoomPos.y <= 0 ? m_BlockDownDoor : m_BlockUpDoor;
            EnvironmentManager.Instance.GenerateBarrelToBlockDoor(currentRoomTransform, (Vector2)currentRoomTransform.position + blockObjectPos);

            return true;
        }

        return false;
    }


    #region Getters
    /*
    public int GetGeneratedRoomNum()
    {
        return m_GeneratedRoomNum;
    }

    public int GetMaxGeneratedRoomNum()
    {
        return m_MaxGeneratedRoomNum;
    }
    */
    #endregion

    #region Setters
    /*
    public void IncrementGeneratedRoomNum()
    {
        m_GeneratedRoomNum++;
    }
    */
    #endregion
}