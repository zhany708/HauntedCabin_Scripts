using System;
using UnityEngine;



#region ���Enum
public enum RoomTypeName
{
    None,

    AllDirection,   //���ܶ�����
    AllHorizontal,  //���Ҷ�����
    AllVertical,    //���¶�����
    OneHorizontal,  //������һ����
    OneVertical,    //������һ����
    AllHorizontalAndOneVertical,    //���Ҷ����ţ�����ֻ��һ��
    OneHorizontalAndAllVertical,    //���¶����ţ�����ֻ��һ��
    OneHorizontalAndOneVertical    //���º����Ҹ���һ����



    /* X��180��ת���£�Y��180��ת���ң�Z��180ͬʱ��ת���º�����
    1. ���ܶ����ţ�����Ҫ��ת��
    2. ���Ҷ����ţ�������ڴ��ã�������Ҫ��ת��
    3. ������һ���ţ��ɺ�����ת��Y��180��
    4. ���¶����ţ�����Ҫ��ת��
    5. ������һ���ţ���������ת��X��180��
    6. ���º����Ҹ���һ���ţ����������������ͺ�����ת��X��180��Y��180��
    7. ���¶��У�����ֻ��һ����������ת��Y��180��
    8. ���Ҷ����ţ�����ֻ��һ����������ת��X��180��
    */
}


[Flags]
public enum DoorFlags   //ͨ��Bit Flag�жϷ��������
{
    None = 0,

    Left = 1 << 0,  //1
    Right = 1 << 1, //2
    Up = 1 << 2,    //4
    Down = 1 << 3  //8
}

[Flags]
public enum CheckFlags      //�жϵ�ǰ�����Ƿ�����Լ��Ƿ�������Χ�ķ���
{
    None = 0,

    Left = 1 << 0,  //1
    Right = 1 << 1, //2
    Up = 1 << 2,    //4
    Down = 1 << 3  //8
}
#endregion


[System.Serializable]
public class RoomType : MonoBehaviour
{
    Transform m_Doors;      //Doors������

    DoorFlags m_DoorFlags;
    CheckFlags m_CheckFlags = CheckFlags.None;      //��ʼ����ʱ�����з���Ĳ�����Ϊfalse


    



    #region Unity�ڲ�����ѭ��
    private void Awake()
    {
        m_Doors = transform.Find("Doors");      //�ҵ�Doors�����壬Ȼ��ͨ������������һѰ���Ƿ��ж�Ӧ�Ĳ���

        m_DoorFlags = DoorFlags.None;       //��Bit Flag��ֵ


        InitDoorFlags();     //��Ϸ��ʼʱ��ʼ���ŵ�����
    }

    private void OnEnable()
    {
        //Debug.Log(gameObject.name + "'s room type is " + GetRoomTypeName() );     //��ʾ��ǰ���������
    }
    #endregion


    #region ö����غ���
    public RoomTypeName GetRoomType()
    {
        
        bool hasLeftDoor = (m_DoorFlags & DoorFlags.Left) != 0;
        bool hasRightDoor = (m_DoorFlags & DoorFlags.Right) != 0;
        bool hasUpDoor = (m_DoorFlags & DoorFlags.Up) != 0;
        bool hasDownDoor = (m_DoorFlags & DoorFlags.Down) != 0;
        

        bool hasHorizontal = hasLeftDoor || hasRightDoor;
        bool hasVertical = hasUpDoor || hasDownDoor;

        if (hasLeftDoor && hasRightDoor && hasUpDoor && hasDownDoor)    //�ȼ���Ƿ�����
        {
            return RoomTypeName.AllDirection;
        }

        else if (hasLeftDoor && hasRightDoor)   //�ټ���Ƿ���������
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

        else if (hasUpDoor && hasDownDoor)      //�ټ���Ƿ���������
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

        else if (hasHorizontal && hasVertical)      //�ټ���Ƿ�������������
        {
            return RoomTypeName.OneHorizontalAndOneVertical;
        }

        else if (hasHorizontal)     //�ټ���Ƿ�ֻ��һ����������
        {
            return RoomTypeName.OneHorizontal;
        }

        else if (hasVertical)
        {
            return RoomTypeName.OneVertical;
        }

        return RoomTypeName.None;   //Ĭ�Ϸ��ؿ�
    }







    private void InitDoorFlags()        //���ݵ�ǰ����ӵ�е�������������
    {
        m_DoorFlags = (IsDoorExist(m_Doors, "LeftDoor") ? DoorFlags.Left : DoorFlags.None) |
                      (IsDoorExist(m_Doors, "RightDoor") ? DoorFlags.Right : DoorFlags.None) |
                      (IsDoorExist(m_Doors, "UpDoor") ? DoorFlags.Up : DoorFlags.None) |
                      (IsDoorExist(m_Doors, "DownDoor") ? DoorFlags.Down : DoorFlags.None);
    }

    private bool IsDoorExist(Transform doors, string checkDoor)     //��鷿������Щ��
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



    public bool HasCheckFlag(CheckFlags flag)       //����Ƿ��м������еķ���
    {
        return (m_CheckFlags & flag) == flag;
    }

    public void SetCheckFlag(CheckFlags flag)       //�����Ѿ����������еķ���
    {
        m_CheckFlags |= flag;
    }
    #endregion


    #region Getters
    public DoorFlags GetDoorFlags()
    {
        return m_DoorFlags;
    }
    #endregion
}