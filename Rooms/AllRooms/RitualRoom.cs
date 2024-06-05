public class RitualRoom : RootRoomController
{
    protected override void OnEnable()
    {
        base.OnEnable();

        //因为仪式房只有一个，所以生成后将仪式房从列表中移除
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(HellsCall.Instance.RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(HellsCall.Instance.RitualRoomName);
        }

        HellsCall.Instance.SetRitualRoomDoorController(doorControllerInsideThisRoom);   //将仪式房的门控制器脚本传给剧本脚本
    }
}