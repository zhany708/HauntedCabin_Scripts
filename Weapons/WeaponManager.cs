using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class WeaponManager : MonoBehaviour        //用于管理武器的加载/销毁等
{
    public static WeaponManager Instance { get; private set; }


    private Transform m_PrimaryWeapon;
    private Transform m_SecondaryWeapon;




    //Dictionary<string, Weapon> m_WeaponDict;      //存放正在使用的武器的字典
    Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();     //预制件缓存字典






    void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

        //赋值主武器和副武器给脚本
        SetupWeaponHolder(ref m_PrimaryWeapon, "PrimaryWeapon");
        SetupWeaponHolder(ref m_SecondaryWeapon, "SecondaryWeapon");
    }





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
        GameObject weaponPrefab = await LoadWeaponAsync(weaponName);
        if (weaponPrefab == null)
        {
            Debug.LogError("Failed to load weapon prefab: " + name);
            return;
        }


        //生成物体，并根据第二个参数决定父物体为主武器还是副武器
        GameObject weaponObject = Instantiate(weaponPrefab, isPrimary ? m_PrimaryWeapon : m_SecondaryWeapon);

        //生成完武器物体后，传递给玩家脚本
        EquipWeaponToPlayer(weaponObject, isPrimary);
    }







    //异步加载
    private async Task<GameObject> LoadWeaponAsync(string name)
    {
        //检查字典里是否有武器，如果有的话直接返回
        if (!m_PrefabDict.TryGetValue(name, out GameObject weaponPrefab))
        {
            //异步加载武器
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            await handle.Task;

            //检查异步加载是否成功
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                weaponPrefab = handle.Result;

                //将武器预制件存进字典
                m_PrefabDict[name] = weaponPrefab;
            }

            else
            {
                Debug.LogError($"Failed to load Weapon prefab: {name}");
                return null;
            }
        }

        return weaponPrefab;
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





    //在Addressables里释放武器，只有这样才能释放内存
    public void ReleaseWeapon(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //检查是否有“克隆”后缀，如果有的话减去后缀。（Clone）刚好有7个字符
            key = key.Substring(0, key.Length - 7);
        }


        if (m_PrefabDict.TryGetValue(key, out GameObject weaponPrefab))
        {
            Addressables.Release(weaponPrefab);

            //从预制件缓存字典中移除武器物体
            m_PrefabDict.Remove(key);

            Debug.Log("Weapon released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This weapon is not loaded yet, cannot release: " + key);
        }
    }
}