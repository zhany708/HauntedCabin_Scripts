//using UnityEngine;



using UnityEngine;

public class RootRoomController : NormalRoomController      //初始房间脚本
{
    #region Unity内部函数
    protected override void Start()
    {
        base.Start();

        if (!RoomManager.Instance.ImportantRoomPos.Contains(transform.position))
        {
            //将该房间的坐标加进重要房间坐标列表，以在重置游戏时不被删除
            RoomManager.Instance.ImportantRoomPos.Add(transform.position);
        }

        //将初始房间在小地图上显示的颜色改成绿色
        MiniMapControllerInsideThisRoom.BaseSprite.color = MiniMapControllerInsideThisRoom.GreenForRootRoom;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        //检查房间是否加进字典（因为初始房间可能因为一些原因在加载时从字典中移除）
        if (!RoomManager.Instance.GeneratedRoomDict.ContainsKey(transform.position))
        {
            RoomManager.Instance.GeneratedRoomDict.Add(transform.position, gameObject);
        }
    }
    #endregion


    #region 其它函数
    public override void ResetGame()
    {
        base.ResetGame();


        //加进字典，防止因为一些原因导致房间没有正确的储存在字典中（所有的初始房间都需要执行此逻辑）
        if (!RoomManager.Instance.GeneratedRoomDict.ContainsKey(transform.position))
        {
            RoomManager.Instance.GeneratedRoomDict.Add(transform.position, gameObject);

            //Debug.Log("We have room: " + RoomManager.Instance.GeneratedRoomDict[transform.position] + "at Vectro2.Zero");
        }      
    }
    #endregion
}