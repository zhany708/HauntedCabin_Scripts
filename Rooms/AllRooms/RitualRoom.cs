public class RitualRoom : RootRoomController
{
    protected override void OnEnable()
    {
        base.OnEnable();

<<<<<<< HEAD
        //因为仪式房只有一个，所以生成后将仪式房的名字从列表中移除
=======
        //因为仪式房只有一个，所以生成后将仪式房从列表中移除
>>>>>>> bb24b688957aaf28d467dd9a5046ca87ffa552a3
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(HellsCall.Instance.RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(HellsCall.Instance.RitualRoomName);
        }

        HellsCall.Instance.SetRitualRoomDoorController(doorControllerInsideThisRoom);   //将仪式房的门控制器脚本传给剧本脚本
    }
}