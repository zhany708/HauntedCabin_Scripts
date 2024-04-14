using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class WeaponInventory        //用于储存武器
{
    private static WeaponInventory m_Instance;
    public static WeaponInventory Instance    //单例模式（整局游戏只存在一个此类的实例）
    {
        get
        {
            if (m_Instance == null)     //第一次检查
            {
                m_Instance = new WeaponInventory();
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
    Dictionary<string, GameObject> m_PrefabDict;     //预制件缓存字典








    private WeaponInventory()
    {
        InitDicts();
    }


    private void InitDicts()
    {
        m_PrefabDict = new Dictionary<string, GameObject>();
    }



    

    //加载武器
    public IEnumerator LoadWeapon(string name, bool isPrimary)
    {
        GameObject weaponPrefab;
        if (!m_PrefabDict.TryGetValue(name, out weaponPrefab))
        {
            //异步加载预制件
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            yield return new WaitUntil(() => handle.IsDone);

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load the weapon prefab: " + name);
                yield break;
            }

            weaponPrefab = handle.Result;
            m_PrefabDict[name] = weaponPrefab;  //将加载的预制件存进字典
        }


        GameObject weaponObject = GameObject.Instantiate(weaponPrefab, isPrimary? PrimaryWeapon : SecondaryWeapon, false);

        //将武器预制件赋值给Player脚本中的主/副武器
        Player player = GameObject.FindObjectOfType<Player>();
        player.SetWeapon(weaponObject.GetComponent<Weapon>(), isPrimary);
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