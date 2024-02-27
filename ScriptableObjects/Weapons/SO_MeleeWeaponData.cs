using UnityEngine;


[CreateAssetMenu(fileName = "newMeleeWeaponData", menuName = "Data/Weapon Data/MeleeWeapon")]

public class SO_MeleeWeaponData : SO_WeaponData
{
    [SerializeField] private MeleeWeaponAttackDetails[] attackDetails;


    public MeleeWeaponAttackDetails[] AttackDetails { get => attackDetails; private set => attackDetails = value; }      //用于其他脚本调用此脚本中的私有变量



    private void OnEnable()
    {
        AmountOfAttack = attackDetails.Length;      //传递武器的攻击次数

        /*
        MovementSpeed = new float[AmountOfAttack];

        for (int i = 0; i < AmountOfAttack; i++)
        {
            MovementSpeed[i] = attackDetails[i].MovementSpeed;      //将攻击性武器数据中的移动补偿速度传递给武器数据中的移动补偿速度
        }
        */
    }
}






[System.Serializable]       //用于在Unity中显示且调整以下参数
public struct MeleeWeaponAttackDetails
{
    public string AttackName;
    //public float MovementSpeed;

    public float DamageAmount;
    public float KnockbackStrength;

    public float CameraShakeIntensity;
    public float CameraShakeDuration;
}
