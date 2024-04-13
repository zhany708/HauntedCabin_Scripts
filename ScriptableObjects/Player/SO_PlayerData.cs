using UnityEngine;


[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class SO_PlayerData : ScriptableObject      //允许通过此脚本创建Asset
{
    [Header("Move State")]
    public float MovementVelocity = 2f;
    public float MaxHealth = 10f;
    public float Defense = 2f;
    public float HitResistance = 1f;
}
