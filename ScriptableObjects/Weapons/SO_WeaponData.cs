using UnityEngine;


//[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Weapon")]

public class SO_WeaponData : ScriptableObject
{
    public int AmountOfAttack { get; protected set; }       //用于在攻击性武器数值脚本中修改


    public string AudioClipName;    //该武器的音效名，用于加载
    public float AudioVolume = 1f;       //武器音效的音量

    //武器攻击时的补偿移动速度。加上只能内部调整，从而防止Unity中重复需要我们调整数值（攻击性武器数据中也有移动补偿速度需要设置）
    //public float[] MovementSpeed { get; protected set; }
}
