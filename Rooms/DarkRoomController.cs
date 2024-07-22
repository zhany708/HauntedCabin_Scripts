using UnityEngine;



public class DarkRoomController : NormalRoomController
{
    public int DarkRoomIndex = -1;      //预兆房间的序列号，用于决定开启哪个剧本




    protected override void Awake()
    {
        base.Awake();

        //检查预兆房间的序列号
        if (DarkRoomIndex < 0)
        {
            Debug.LogError("DarkRoomIndex is not assigned in the: " + gameObject.name);
            return;
        }
    }
}