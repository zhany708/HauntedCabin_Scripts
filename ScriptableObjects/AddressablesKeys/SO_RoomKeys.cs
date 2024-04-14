using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newRoomKeys", menuName = "Data/Room Data/Room Keys")]
public class SO_RoomKeys : ScriptableObject
{
    //������Ҫ���б����򲻷����������

    //��ʼ����
    [Header("Root Room")]
    public List<string> RootRoomKeys;

    //һ¥
    [Header("First Floor")]
    public List<string> FirstFloorRoomKeys;

    //��¥
    [Header("Second Floor")]
    public List<string> SecondFloorRoomKeys;

    //¥��
    [Header("Top Floor")]
    public List<string> TopFloorRoomKeys;

    //������
    [Header("Basement")]
    public List<string> BasementRoomKeys;
}