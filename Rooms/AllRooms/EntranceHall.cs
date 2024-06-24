using UnityEngine;



public class EntranceHall : RootRoomController      //入口大堂脚本
{
    public static EntranceHall Instance { get; private set; }







    protected override void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            //当重复生成入口大堂时，删除重复的
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        RoomManager.Instance.ImportantRoomPos.Add(transform.position);      //将房间坐标加进不可删除列表，防止重置游戏时被删除
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