public class RitualRoom : RootRoomController
{
    protected override void OnEnable()
    {
        base.OnEnable();

        //��Ϊ��ʽ��ֻ��һ�����������ɺ���ʽ�������ִ��б����Ƴ�
        if (RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Contains(HellsCall.Instance.RitualRoomName) )
        {
            RoomManager.Instance.RoomKeys.FirstFloorRoomKeys.Remove(HellsCall.Instance.RitualRoomName);
        }

        HellsCall.Instance.SetRitualRoomDoorController(doorControllerInsideThisRoom);   //����ʽ�����ſ������ű������籾�ű�
    }
}