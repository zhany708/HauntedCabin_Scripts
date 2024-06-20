public class RitualRoom : RootRoomController        //仪式房脚本
{
    public static RitualRoom Instance { get; private set; }



    protected override void Awake()
    {      
        //单例模式
        if (Instance != null && Instance != this)
        {
            //当重复生成仪式房时，删除重复的，同时生成通用房间以代替
            Destroy(gameObject);
          
            RoomManager.Instance.GenerateRoomAtThisPos(transform.position, RoomManager.Instance.RoomKeys.GenericRoomKey);
        }

        else
        {
            Instance = this;
        }

        base.Awake();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        //因为仪式房只有一个，所以生成后将仪式房从Key中移除
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(HellsCall.Instance.RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(HellsCall.Instance.RitualRoomName);
        }

        HellsCall.Instance.SetRitualRoomDoorController(DoorControllerInsideThisRoom);   //将仪式房的门控制器脚本传给剧本脚本
    }
}