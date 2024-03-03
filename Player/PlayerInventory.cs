using System.Collections.Generic;
using UnityEngine;



public class WeaponConst    //用于存储武器的名称
{
    public const string Dagger = "Dagger";    //玩家状态栏。在HealthBar脚本里初始化
    public const string Shotgun = "Shotgun";    //进入二阶段文字
}




public class PlayerInventory : MonoBehaviour        //用于储存武器
{
    public GameObject[] PrimaryWeapon;
    public GameObject[] SecondaryWeapon;


    Dictionary<string, Weapon> m_WeaponDict;      //存放正在使用的武器的字典
    Dictionary<string, string> m_PathDict;       //使用字典存储武器的配置路径表
    Dictionary<string, GameObject> m_PrefabDict;     //预制件缓存字典




    private void Awake()
    {
        InitDicts();
    }



    private void InitDicts()
    {
        m_WeaponDict = new Dictionary<string, Weapon>();
        m_PrefabDict = new Dictionary<string, GameObject>();

        m_PathDict = new Dictionary<string, string>()   //初始化所有界面的路径
        {
            {WeaponConst.Dagger, "" },
            {WeaponConst.Shotgun, "" }
        };
    }


    public Weapon LoadWeapon(string name)
    {
        return null;
    }
}

