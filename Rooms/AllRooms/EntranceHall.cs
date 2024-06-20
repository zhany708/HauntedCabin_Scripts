using UnityEngine;


public class EntranceHall : RootRoomController      //入口大堂脚本
{
    protected override void Start()
    {
        base.Start();

        RoomManager.Instance.ImportantRoomPos.Add(transform.position);
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Player") && MainDoorController.Instance.DoOpenMainDoor)       //当大门允许打开时，在玩家进入入口大堂后再打开大门
        {
            MainDoorController.Instance.OpenMainDoor();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        //只有在大门没开时，才会进行正常的逻辑
        if (other.CompareTag("Player") && !MainDoorController.Instance.DoOpenMainDoor)
        {
            base.OnTriggerExit2D(other);
        }
    }
}