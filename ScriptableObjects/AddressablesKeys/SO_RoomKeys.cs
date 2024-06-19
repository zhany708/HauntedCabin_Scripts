using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newRoomKeys", menuName = "Data/Room Data/Room Keys")]
public class SO_RoomKeys : ScriptableObject
{
    //房间需要用列表，否则不方便随机生成

    //初始房间
    [Header("Root Room")]
    public List<string> RootRoomKeys;

    //一楼
    [Header("First Floor")]
    public List<string> FirstFloorRoomKeys;

    //二楼
    [Header("Second Floor")]
    public List<string> SecondFloorRoomKeys;

    //楼顶
    [Header("Top Floor")]
    public List<string> TopFloorRoomKeys;

    //地下室
    [Header("Basement")]
    public List<string> BasementRoomKeys;



    //通用房间（当因为逻辑错误需要重新生成一些房间时，同一调用下面这个房间）
    [Header("Generic Room")]
    public string GenericRoomKey;
}