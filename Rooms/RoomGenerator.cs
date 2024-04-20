using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;



public class RoomGenerator : MonoBehaviour
{
    public SO_RoomKeys RoomKeys;
    public GameObject BlockDoorBarrel;
    public Transform FatherOfAllRooms;      //所有生成的房间的父物体，为了整洁美观
    public LayerMask roomLayerMask;
    

    //储存加载过的房间的坐标和物体，用于检查是否有连接的门
    public Dictionary<Vector2, GameObject> GeneratedRoomDict = new Dictionary<Vector2, GameObject>();



    public float MaximumXPos = 35f;
    public float MaximumYPos = 35f;


    //储存加载过的房间的名字和物体，用于异步加载的检查
    Dictionary<string, GameObject> m_RoomDict;


    int m_RandomGeneratedNum = -1;          //随机生成的数（用于新的房间生成的索引）


    Transform m_CurrentRoomTransform;

    //用于生成挡住玩家进入门的障碍物的坐标
    Vector2 m_BlockUpDoor = new Vector2(0f, 3.7f);
    Vector2 m_BlockDownDoor = new Vector2(0f, -4.65f);
    Vector2 m_BlockLeftDoor = new Vector2(-7.55f, -0.7f);
    Vector2 m_BlockRightDoor = new Vector2(7.55f, -0.7f);




    private void Awake()
    {
        //初始化字典
        m_RoomDict = new Dictionary<string, GameObject>();
    }




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
                GenerateSuitableRoom(newRoomPos, neededDoorName);     //如果所有条件都满足，则生成合适的房间
            }

            else
            {
                //当目标生成位置已经有房间时，检查那个房间是否有连接当前房间的门，没有的话就放障碍物阻止玩家通过
                CheckRequiredDoorAtOverlapPosition(newRoomPos, currentRoomTransform, neededDoorName);

                //Debug.Log("Cannot generate room at this repeated position: " + newRoomPos);
            }
        }
    }


    private bool IsPositionEmpty(Vector2 positionToCheck, Vector2 roomSize)
    {
        //第一个参数为中心点，第二个参数为正方形大小，第三个参数为角度，第四个参数为检测的目标层级
        Collider2D overlapCheck = Physics2D.OverlapBox(positionToCheck, roomSize, 0f, roomLayerMask);
        return overlapCheck == null;
    }


    private bool CanGenerateRoomAtPosition(Vector2 curremtRoomPos, Vector2 offset)
    {
        Vector2 newRoomPos = curremtRoomPos + offset;

        //以目标点（上面的newRoomPos）为中心点开始的长方形大小
        Vector2 roomSize = new Vector2(15f, 10f);

        //检查这个长方形大小里是否有房间图层
        if (IsPositionEmpty(newRoomPos, roomSize))
        {
            return true;
        }

        else
        {
            return false;
        }
    }



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
                    Instantiate(BlockDoorBarrel, (Vector2)currentRoomTransform.position + blockObjectPos, Quaternion.identity, currentRoomTransform);
                }
            }
        }
    }



    private async Task<GameObject> LoadRoomAsync(string name)
    {
        //如果字典里已经有房间了，则直接返回
        if (m_RoomDict.TryGetValue(name, out GameObject val))
        {
            return val;
        }


        //异步加载房间后，检查物体是否存在
        GameObject loadedRoomObject = await Addressables.LoadAssetAsync<GameObject>(name).Task;
        if (loadedRoomObject != null)
        {
            //将房间物体储存进字典
            m_RoomDict[name] = loadedRoomObject;

            return loadedRoomObject;
        }

        else
        {
            Debug.LogError("Failed to load room: " + name);
            return null;
        }
    }


    //在Addressables里释放房间，只有这样才能释放内存
    public void ReleaseRoom(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
            key = key.Substring(0, key.Length - 7);
        }


        if (m_RoomDict.TryGetValue(key, out GameObject roomPrefab))
        {
            Addressables.Release(roomPrefab);

            //从字典中移除房间物体
            m_RoomDict.Remove(key);

            Debug.Log("Room released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This room is not loaded yet, cannot release: " + key);
        }
    }



    private async void GenerateSuitableRoom(Vector2 newRoomPos, string neededDoorName)
    {
        GameObject newRoom = null;
        bool isRoomPlaced = false;
        int attemptCount = 0;
        const int maxAttempts = 50;     //最大尝试次数

        while (!isRoomPlaced && attemptCount < maxAttempts)     //生成房间次数大于200次后强制返回，防止出现无限循环
        {
            attemptCount++;

            m_RandomGeneratedNum = UnityEngine.Random.Range(0, RoomKeys.FirstFloorRoomKeys.Count);       //随机生成房间的索引   ToDo：需要决定生成哪一层的房间


            //确认随机索引后尝试异步加载房间
            try
            {
                GameObject loadedRoom = await LoadRoomAsync(RoomKeys.FirstFloorRoomKeys[m_RandomGeneratedNum] );       //异步加载事件
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
                ReleaseRoom(newRoom.name);
            }
        }

        if ( !isRoomPlaced )        //如果超过最大尝试次数后依然没有合适的房间，则实施一些功能
        {
            Debug.Log("Failed to place a suitable room after " + maxAttempts + " attempts!");
        }
    }



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


    private bool CheckIfBreakMaximumPos(Transform currentRoomTransform, Vector2 newRoomPos)
    {
        //如果要生成的房间的坐标超出了限制墙壁时，则在进入那个房间的门口处生成障碍物阻止玩家前进
        if (Mathf.Abs(newRoomPos.x) >= MaximumXPos)
        {
            //Debug.Log("Cannot generate new room at this position: " + newRoomPos);

            Vector2 blockObjectPos = newRoomPos.x <= 0 ? m_BlockLeftDoor : m_BlockRightDoor;
            Instantiate(BlockDoorBarrel, (Vector2)currentRoomTransform.position + blockObjectPos, Quaternion.identity, currentRoomTransform);

            return true;
        }

        else if (Mathf.Abs(newRoomPos.y) >= MaximumYPos)
        {
            Vector2 blockObjectPos = newRoomPos.y <= 0 ? m_BlockDownDoor : m_BlockUpDoor;
            Instantiate(BlockDoorBarrel, (Vector2)currentRoomTransform.position + blockObjectPos, Quaternion.identity, currentRoomTransform);

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