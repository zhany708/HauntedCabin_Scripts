using System.Threading.Tasks;
using UnityEngine;



public class WeaponManager : ManagerTemplate<WeaponManager>        //用于管理武器的加载/销毁等
{
    private Transform m_PrimaryWeapon;
    private Transform m_SecondaryWeapon;




    //Dictionary<string, Weapon> m_WeaponDict;      //存放正在使用的武器的字典






    protected override void Awake()
    {
        base.Awake();

        //赋值主武器和副武器给脚本
        SetupWeaponHolder(ref m_PrimaryWeapon, "PrimaryWeapon");
        SetupWeaponHolder(ref m_SecondaryWeapon, "SecondaryWeapon");
    }





    //设置脚本中主副武器的坐标
    private void SetupWeaponHolder(ref Transform weaponHolder, string weaponHolderName)
    {
        //尝试寻找游戏中用于第二个参数的名字的物体，如果没找到的话就新建一个
        GameObject weaponHolderObject = GameObject.Find(weaponHolderName) ?? new GameObject(weaponHolderName);


        //建完后寻找Player脚本并将玩家设置为武器的父物体
        Player player = FindObjectOfType<Player>();
        if (player)
        {
            weaponHolderObject.transform.SetParent(player.transform);
        }
        

        weaponHolder = weaponHolderObject.transform;
    }




    public async Task LoadWeapon(string weaponName, bool isPrimary)
    {
        //异步加载，随后检查是否加载成功
        GameObject weaponPrefab = await LoadPrefabAsync(weaponName);
        if (weaponPrefab == null)
        {
            Debug.LogError("Failed to load weapon prefab: " + name);
            return;
        }

        //如果因为场景加载等原因导致玩家物体被删除过，就重新获取主副武器
        if (m_PrimaryWeapon == null || m_SecondaryWeapon == null)
        {
            //赋值主武器和副武器给脚本
            SetupWeaponHolder(ref m_PrimaryWeapon, "PrimaryWeapon");
            SetupWeaponHolder(ref m_SecondaryWeapon, "SecondaryWeapon");
        }

        //生成物体，并根据第二个参数决定父物体为主武器还是副武器
        GameObject weaponObject = Instantiate(weaponPrefab, isPrimary ? m_PrimaryWeapon : m_SecondaryWeapon);

        //生成完武器物体后，传递给玩家脚本
        EquipWeaponToPlayer(weaponObject, isPrimary);
    }




    //将新武器预制件赋值给Player脚本中的主/副武器
    private void EquipWeaponToPlayer(GameObject weaponObject, bool isPrimary)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (player)
        {
            player.SetWeapon(weaponObject.GetComponent<Weapon>(), isPrimary);
        }
    }
}