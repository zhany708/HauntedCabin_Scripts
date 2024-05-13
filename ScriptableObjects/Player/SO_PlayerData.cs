using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class SO_PlayerData : ScriptableObject      //允许通过此脚本创建Asset
{
    [Header("Movement Values")]
    public float MovementVelocity = 2f;     //玩家移速的默认值为2

    [Header("Status Values")]
    public float MaxHealth = 10f;   //最大生命值
    public float Strength = 5f;     //力量（四个属性的默认值都为5）
    public float Speed = 5f;        //速度
    public float Sanity = 5f;       //神志
    public float Knowledge = 5f;    //知识
    public float Defense = 2f;      //防御

    [Header("Hit Values")]
    public float HitResistance = 1f;
}