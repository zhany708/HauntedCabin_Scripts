using System.Collections.Generic;
using UnityEngine;





public class RoomGenerator : MonoBehaviour
{
    public GameObject[] AllRooms;

    public Transform FatherOfAllRooms;      //所有生成的房间的父物体，为了整洁美观

    public HashSet<Vector2> GeneratedRoomPos = new HashSet<Vector2>();      //HashSet在性能上要优于List（不在乎顺序的情况下，且它不能储存重复的东西）
    //public List<Vector2> GeneratedRoomPos = new List<Vector2>();      //用于Debug，因为Unity里看不到HashSet


    int m_GeneratedRoomNum = 0;
    const int m_MaxGeneratedRoomNum = 20;




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


    


    private void GenerateSuitableRoom(Vector2 newRoomPos, string neededDoorName)
    {
        bool isRoomPlaced = false;
        int attemptCount = 0;
        const int maxAttempts = 200;     //最大尝试次数

        while (!isRoomPlaced && attemptCount < maxAttempts)     //生成房间次数大于200次后强制返回，防止出现无限循环
        {
            attemptCount++;

            int randomRoomNum = Random.Range(0, AllRooms.Length);       //随机生成房间的索引

            GameObject newRoom = Instantiate(AllRooms[randomRoomNum], newRoomPos, Quaternion.identity, FatherOfAllRooms);
            RoomType newRoomType = newRoom.GetComponent<RoomType>();

            //先检查是否直接有需要的门，如果没有则通过旋转之后再次检查
            if (HasRequiredDoor(newRoomType, neededDoorName) || TryRotateRoomToMatchDoor(newRoomType, neededDoorName) )
            {
                isRoomPlaced = true;
            }
            else
            {
                Destroy(newRoom);
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