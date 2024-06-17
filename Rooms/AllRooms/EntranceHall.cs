using UnityEngine;


public class EntranceHall : RootRoomController      //入口大堂脚本
{
    MainDoorController m_MainDoor;      //大门控制器


    

    
    protected override void Awake()
    {
        base.Awake();

        m_MainDoor = GetComponentInChildren<MainDoorController>();
    }
    

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (m_MainDoor.DoOpenMainDoor)       //当大门允许打开时，在玩家进入入口大堂后再打开大门
        {
            m_MainDoor.OpenMainDoor();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        //只有在大门没开时，才会进行正常的逻辑
        if (!m_MainDoor.DoOpenMainDoor)
        {
            base.OnTriggerExit2D(other);
        }
    }
}