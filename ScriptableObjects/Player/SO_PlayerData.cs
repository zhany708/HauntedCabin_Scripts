using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class SO_PlayerData : ScriptableObject      //允许通过此脚本创建Asset
{
    [Header("Movement Values")]
    public float MovementVelocity = 2f;

    [Header("Status Values")]
    public float MaxHealth = 10f;   //最大生命值
    public float Strength = 1f;     //力量
    public float Speed = 1f;        //速度
    public float Sanity = 1f;       //神志
    public float Knowledge = 1f;    //知识
    public float Defense = 2f;      //防御

    [Header("Hit Values")]
    public float HitResistance = 1f;
}