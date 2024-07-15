using UnityEngine;



[CreateAssetMenu(fileName = "newAudioClipKeys", menuName = "Data/Audio Data/Clip Keys")]
public class SO_AudioClipKeys : ScriptableObject
{
    //BGM
    [Header("BGM Music")]
    //休闲音乐
    public string StopForAMoment;           //用于一楼场景
    public string Heaven;                   //用于剧本《地狱的呼唤》胜利后

    //恐怖音乐
    public string MyVeryOwnDeadShip;        //用于主菜单

    //紧张音乐
    public string FireEscape;               //用于剧本《地狱的呼唤》




    //某些音效因为在创建独立的ScriptableObject时已经储存音效名字了，因此不会从这里调取名字！

    //武器音效
    [Header("Weapon Audio")]
    public string ShotgunKey;                       //霰弹枪的射击声


    //事件音效
    [Header("Event Audio")]
    public string PhoneRingKey;                     //电话铃声响起
    public string AnswerPhoneKey;                   //接电话的声音
    public string FemaleWhisperKey;                 //老妇人的呢喃



    //游戏背景界面相关的音效
    [Header("GameBackgroundPanel related")]
    public string RainingKey;                       //下雨
    public string MainDoorCloseKey;                 //大门关闭

    //这两个音效暂时不需要
    //public string MonsterRoarKey;     
    //public string ChildScreamKey;


    //地狱的呼唤背景界面相关的音效
    [Header("HellsCallBackground related")]
    public string FireBurningKey;                   //火焰灼烧
}