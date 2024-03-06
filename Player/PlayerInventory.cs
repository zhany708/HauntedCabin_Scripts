using System.Collections.Generic;
using UnityEngine;



public class WeaponConst    //用于存储武器的名称
{
    public const string Dagger = "Dagger";    //初始匕首
    public const string Shotgun = "Shotgun";    //霰弹枪
}




public class PlayerInventory        //用于储存武器
{
    private static PlayerInventory m_Instance;
    public static PlayerInventory Instance    //单例模式（整局游戏只存在一个此类的实例）
    {
        get
        {
            if (m_Instance == null)     //第一次检查
            {
                m_Instance = new PlayerInventory();
            }
            return m_Instance;
        }
    }


    private Transform m_PrimaryWeapon;
    public Transform PrimaryWeapon
    {
        get
        {
            if (m_PrimaryWeapon == null)
            {
                GameObject primaryWeapon = GameObject.Find("PrimaryWeapon");

                if (primaryWeapon != null)
                {
                    m_PrimaryWeapon = primaryWeapon.transform;
                }
                else
                {
                    m_PrimaryWeapon = new GameObject("PrimaryWeapon").transform;

                    Player player = GameObject.FindObjectOfType<Player>();     //将Player设为父物体
                    m_PrimaryWeapon.SetParent(player.transform);
                }
            }
            return m_PrimaryWeapon;
        }
    }

    private Transform m_SecondaryWeapon;
    public Transform SecondaryWeapon
    {
        get
        {
            if (m_SecondaryWeapon == null)
            {
                GameObject secondaryWeapon = GameObject.Find("SecondaryWeapon");

                if (secondaryWeapon != null)
                {
                    m_SecondaryWeapon = secondaryWeapon.transform;
                }
                else
                {
                    m_SecondaryWeapon = new GameObject("SecondaryWeapon").transform;

                    Player player = GameObject.FindObjectOfType<Player>();     //将Player设为父物体
                    m_SecondaryWeapon.SetParent(player.transform);
                }
            }
            return m_SecondaryWeapon;
        }
    }



    //Dictionary<string, Weapon> m_WeaponDict;      //存放正在使用的武器的字典
    Dictionary<string, string> m_PathDict;       //使用字典存储武器的配置路径表
    Dictionary<string, GameObject> m_PrefabDict;     //预制件缓存字典








    private PlayerInventory()
    {
        InitDicts();
    }


    private void InitDicts()
    {
        m_PrefabDict = new Dictionary<string, GameObject>();

        m_PathDict = new Dictionary<string, string>()   //初始化所有界面的路径
        {
            {WeaponConst.Dagger, "Dagger/Dagger" },
            {WeaponConst.Shotgun, "Guns/Shotgun" }
        };
    }




    public Weapon LoadWeapon(string name, bool isPrimary)
    {
        Weapon weapon = null;
        GameObject weaponPrefab = null;
        GameObject weaponObject = null;


        //检查缓存的预制件中是否已经有要生成的武器，如果有则直接获取，无须进行下面的检查
        if (m_PrefabDict.TryGetValue(name, out weaponPrefab))
        {
            if (isPrimary)
            {
                weaponObject = GameObject.Instantiate(weaponPrefab, PrimaryWeapon, false);    //生成出来，并使它成为主武器的子物体
            }
            else
            {
                weaponObject = GameObject.Instantiate(weaponPrefab, SecondaryWeapon, false);    //生成出来，并使它成为副武器的子物体
            }


            weapon = weaponObject.GetComponent<Weapon>();
            return weapon;
        }





        //检查武器是否有路径配置
        string path = "";

        if (!m_PathDict.TryGetValue(name, out path))
        {
            Debug.LogError("Something wrong with the name or path of this panel: " + name);
            return null;
        }


        //使用缓存的预制件
        if (!m_PrefabDict.TryGetValue(name, out weaponPrefab))
        {
            string realPath = "Prefab/Weapons/" + path;    //如果没有被加载过，则加载出来并放入缓存字典

            weaponPrefab = Resources.Load<GameObject>(realPath);     //通过Load函数从Assets中寻找资源赋值（必须在Resources文件夹下面）
            m_PrefabDict.Add(name, weaponPrefab);    //加入存放武器预制件的字典
        }


        if (isPrimary)
        {
            weaponObject = GameObject.Instantiate(weaponPrefab, PrimaryWeapon, false);    //生成出来，并使它成为主武器的子物体
        }
        else
        {
            weaponObject = GameObject.Instantiate(weaponPrefab, SecondaryWeapon, false);    //生成出来，并使它成为副武器的子物体
        }


        weapon = weaponObject.GetComponent<Weapon>();
        return weapon;
    }
}

