using UnityEngine;



public class EntranceHall : RootRoomController      //入口大堂脚本
{
    public static EntranceHall Instance { get; private set; }






    #region Unity内部函数
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

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        base.Awake();
    }

    protected override void Start()
    {
        //检查该房间是否是唯一保留的那个
        if (Instance == this)
        {
            base.Start();           //将房间加进RoomManager的GeneratedRoomDict字典
           
            if (!RoomManager.Instance.ImportantRoomPos.Contains(transform.position))
            {
                //将该房间的坐标加进重要房间坐标列表，以在重置游戏时不被删除
                RoomManager.Instance.ImportantRoomPos.Add(transform.position);
            }
        }
    }

    protected override void OnDisable()
    {
        //只有含Instance的本房间取消激活时，才会从字典中移除当前房间的坐标
        if (Instance == this)
        {
            //暂时先不给入口大堂从字典中移除的机会，防止小地图无法正常显示
            //base.OnDisable();
        }
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        
        //当大门允许打开时，在玩家进入入口大堂后再打开大门
        if (other.CompareTag("Player") && MainDoorController.Instance.DoOpenMainDoor)   
        {
            MainDoorController.Instance.OpenMainDoor();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        //只有在大门没开时，才会进行正常的逻辑（房间变暗等）
        if (other.CompareTag("Player") && !MainDoorController.Instance.DoOpenMainDoor)
        {
            base.OnTriggerExit2D(other);
        }
    }
    #endregion


    #region 其它函数
    public override void ResetGame()
    {
        base.ResetGame();

        MainDoorController.Instance.CloseMainDoor();                //重置游戏时关闭大门
        MainDoorController.Instance.SetDoOpenMainDoor(false);       //关闭大门的同时重置布尔


        //检查该房间是否是唯一保留的那个
        if (Instance == this)
        {
            //加进字典，防止因为一些原因导致房间没有正确的储存在字典中（所有的初始房间都需要执行此逻辑）
            if (!RoomManager.Instance.GeneratedRoomDict.ContainsKey(transform.position))
            {
                RoomManager.Instance.GeneratedRoomDict.Add(transform.position, gameObject);

                //Debug.Log("We have room: " + RoomManager.Instance.GeneratedRoomDict[transform.position] + "at Vectro2.Zero");
            }
        }
    }
    #endregion
}