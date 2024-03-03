using UnityEngine;


[CreateAssetMenu(fileName = "newGunWeaponData", menuName = "Data/Weapon Data/GunWeapon")]
public class SO_GunData : SO_WeaponData
{
    [SerializeField] private GunAttackDetails m_AttackDetail;

    public GunAttackDetails AttackDetail { get => m_AttackDetail; private set => m_AttackDetail = value; }      //用于其他脚本调用此脚本中的私有变量
}



[System.Serializable]       //用于在Unity中显示且调整以下参数
public struct GunAttackDetails
{
    public float DamageAmount;

    public float KnockbackStrength;

    public float AttackCooldownTime;
}
