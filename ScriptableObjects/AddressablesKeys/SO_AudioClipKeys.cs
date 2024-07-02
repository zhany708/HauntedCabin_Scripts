using UnityEngine;



[CreateAssetMenu(fileName = "newAudioClipKeys", menuName = "Data/Audio Data/Clip Keys")]
public class SO_AudioClipKeys : ScriptableObject
{
    //BGM
    [Header("BGM Music")]
    //休闲音乐
    public string StopForAMoment;   

    //恐怖音乐
    public string MyVeryOwnDeadShip;


    //某些音效因为在创建独立的ScriptableObject时已经储存音效名字了，因此不会从这里调取名字！

    //武器音效
    [Header("Weapon Audio")]
    public string ShotgunKey;


    //事件音效
    [Header("Event Audio")]
    public string PhoneRingKey;
    public string AnswerPhoneKey;
}