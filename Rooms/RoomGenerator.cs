using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;





public class RoomGenerator : MonoBehaviour
{
    public SO_RoomKeys RoomKeys;

    public Transform FatherOfAllRooms;      //所有生成的房间的父物体，为了整洁美观

    public HashSet<Vector2> GeneratedRoomPos = new HashSet<Vector2>();      //HashSet在性能上要优于List（不在乎顺序的情况下，且它不能储存重复的东西）
    //public List<Vector2> GeneratedRoomPos = new List<Vector2>();      //用于Debug，因为Unity里看不到HashSet



    //储存加载过的房间
    Dictionary<string, GameObject> m_RoomDict;


    int m_GeneratedRoomNum = 0;             //生成了多少房间
    const int m_MaxGeneratedRoomNum = 20;   //最多可以生成的房间数

    int m_RandomGeneratedNum = -1;          //随机生成的数（用于新的房间生成的索引）







    private void Awake()
    {
        //初始化字典
        m_RoomDict = new Dictionary<string, GameObject>();
    }


    public void GenerateRoom(Transform currentRoom, RoomType currentRoomType)
    {
        DoorFlags currentDoorFlags = currentRoomType.GetDoorFlags();

        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Left) != 0, new Vector2(-19.2f, 0), "RightDoor");
        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Right) != 0, new Vector2(19.2f, 0), "LeftDoor");
        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Up) != 0, new Vector2(0, 10.8f), "DownDoor");
        GenerateRoomInDirection(currentRoom, (currentDoorFlags & DoorFlags.Down) != 0, new Vector2(0, -10.8f), "UpDoor");

        currentRoom.GetComponent<RootRoomController>().SetHasGeneratorRoom(true);
    }

    private void GenerateRoomInDirection(Transform currentRoomPos, bool hasDoor, Vector2 offset, string requiredDoor)
    {

        if (hasDoor)
        {
            Vector2 roomPos = (Vector2)currentRoomPos.transform.position + offset;

            if (CheckOverlapPosition(roomPos))  return;     //如果有坐标重复，则返回（不生成房间）

            GenerateSuitableRoom(roomPos, requiredDoor);     //如果所有条件都满足，则生成合适的房间
        }
    }


    private bool CheckOverlapPosition(Vector2 checkPos)     //检查坐标是否重复
    {
        return GeneratedRoomPos.Contains(checkPos);
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
        const int maxAttempts = 200;     //最大尝试次数

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

            //先检查是否直接有需要的门，如果没有则通过旋转之后再次检查
            if (HasRequiredDoor(newRoomType, neededDoorName) || TryRotateRoomToMatchDoor(newRoomType, neededDoorName) )
            {
                isRoomPlaced = true;
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

    private bool TryRotateRoomToMatchDoor(RoomType roomType, string neededDoorName)
    {
        roomType.RotateRoom(neededDoorName);    //旋转房间

        if (HasRequiredDoor(roomType, neededDoorName))     //进行旋转之后，再次检查门的布尔
        {
            roomType.SetIsRotate(true);     //设置isRotate布尔为真，防止房间的Awake函数重新设置4个门的布尔

            return true;
        }
        
        return false;
    }


    #region Getters
    public int GetGeneratedRoomNum()
    {
        return m_GeneratedRoomNum;
    }

    public int GetMaxGeneratedRoomNum()
    {
        return m_MaxGeneratedRoomNum;
    }
    #endregion

    #region Setters
    public void IncrementGeneratedRoomNum()
    {
        m_GeneratedRoomNum++;
    }
    #endregion
}