using System;
using UnityEngine;




public enum RoomTypeName
{
    None,

    AllDirection,   //四周都有门
    AllHorizontal,  //左右都有门
    AllVertical,    //上下都有门
    OneHorizontal,  //左右有一个门
    OneVertical,    //上下有一个门
    AllHorizontalAndOneVertical,    //左右都有门，上下只有一个
    OneHorizontalAndAllVertical,    //上下都有门，左右只有一个
    OneHorizontalAndOneVertical    //上下和左右各有一个门
}


[Flags]
public enum DoorFlags   //通过Bit Flag判断房间的种类
{
    None = 0,

    Left = 1 << 0,  //1
    Right = 1 << 1, //2
    Up = 1 << 2,    //4
    Down = 1 << 3  //8
}




[System.Serializable]
public class RoomType : MonoBehaviour
{
    Transform m_Doors;      //Doors子物体

    DoorFlags m_DoorFlags;
    


    /* X轴180翻转上下，Y轴180翻转左右，Z轴180同时翻转上下和左右
    1. 四周都有门（不需要旋转）
    2. 左右都有门（比如入口大堂）（不需要旋转）
    3. 左右有一个门（可横向旋转，Y轴180）
    4. 上下都有门（不需要旋转）
    5. 上下有一个门（可竖向旋转，X轴180）
    6. 上下和左右各有一个门（比如厨房）（竖向和横向旋转，X轴180，Y轴180）
    7. 上下都有，左右只有一个（横向旋转，Y轴180）
    8. 左右都有门，上下只有一个（竖向旋转，X轴180）
     */




    private void Awake()
    {
        m_Doors = transform.Find("Doors");      //找到Doors子物体，然后通过该子物体逐一寻找是否有对应的侧门

        m_DoorFlags = DoorFlags.None;       //给Bit Flag赋值


        SetDoorFlags();     //游戏开始时赋值四个布尔       
    }

    private void OnEnable()
    {
        //Debug.Log(gameObject.name + "'s room type is " + GetRoomTypeName() );     //显示当前房间的种类
    }



    


    public RoomTypeName GetRoomType()
    {
        
        bool hasLeftDoor = (m_DoorFlags & DoorFlags.Left) != 0;
        bool hasRightDoor = (m_DoorFlags & DoorFlags.Right) != 0;
        bool hasUpDoor = (m_DoorFlags & DoorFlags.Up) != 0;
        bool hasDownDoor = (m_DoorFlags & DoorFlags.Down) != 0;
        

        bool hasHorizontal = hasLeftDoor || hasRightDoor;
        bool hasVertical = hasUpDoor || hasDownDoor;

        if (hasLeftDoor && hasRightDoor && hasUpDoor && hasDownDoor)    //先检查是否都有门
        {
            return RoomTypeName.AllDirection;
        }

        else if (hasLeftDoor && hasRightDoor)   //再检查是否都有左右门
        {
            if (hasUpDoor || hasDownDoor)
            {
                return RoomTypeName.AllHorizontalAndOneVertical;
            }
            else
            {
                return RoomTypeName.AllHorizontal;
            }
        }

        else if (hasUpDoor && hasDownDoor)      //再检查是否都有上下门
        {
            if (hasLeftDoor || hasRightDoor)
            {
                return RoomTypeName.OneHorizontalAndAllVertical;
            }
            else
            {
                return RoomTypeName.AllVertical;
            }
        }

        else if (hasHorizontal && hasVertical)      //再检查是否两个方向都有门
        {
            return RoomTypeName.OneHorizontalAndOneVertical;
        }

        else if (hasHorizontal)     //再检查是否只有一个方向有门
        {
            return RoomTypeName.OneHorizontal;
        }

        else if (hasVertical)
        {
            return RoomTypeName.OneVertical;
        }

        return RoomTypeName.None;   //默认返回空
    }







    private void SetDoorFlags()
    {
        m_DoorFlags = (IsDoorExist(m_Doors, "LeftDoor") ? DoorFlags.Left : DoorFlags.None) |
                      (IsDoorExist(m_Doors, "RightDoor") ? DoorFlags.Right : DoorFlags.None) |
                      (IsDoorExist(m_Doors, "UpDoor") ? DoorFlags.Up : DoorFlags.None) |
                      (IsDoorExist(m_Doors, "DownDoor") ? DoorFlags.Down : DoorFlags.None);
    }

    private bool IsDoorExist(Transform doors, string checkDoor)
    {
        Transform door = doors.Find(checkDoor);

        if (door != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }





    #region Getters
    public DoorFlags GetDoorFlags()
    {
        return m_DoorFlags;
    }
    #endregion
}