using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class WeaponInventory        //���ڴ�������
{
    private static WeaponInventory m_Instance;
    public static WeaponInventory Instance    //����ģʽ��������Ϸֻ����һ�������ʵ����
    {
        get
        {
            if (m_Instance == null)     //��һ�μ��
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

                    Player player = GameObject.FindObjectOfType<Player>();     //��Player��Ϊ������
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

                    Player player = GameObject.FindObjectOfType<Player>();     //��Player��Ϊ������
                    m_SecondaryWeapon.SetParent(player.transform);
                }
            }
            return m_SecondaryWeapon;
        }
    }



    //Dictionary<string, Weapon> m_WeaponDict;      //�������ʹ�õ��������ֵ�
    Dictionary<string, GameObject> m_PrefabDict;     //Ԥ�Ƽ������ֵ�








    private WeaponInventory()
    {
        InitDicts();
    }


    private void InitDicts()
    {
        m_PrefabDict = new Dictionary<string, GameObject>();
    }



    

    //��������
    public IEnumerator LoadWeapon(string name, bool isPrimary)
    {
        GameObject weaponPrefab;
        if (!m_PrefabDict.TryGetValue(name, out weaponPrefab))
        {
            //�첽����Ԥ�Ƽ�
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            yield return new WaitUntil(() => handle.IsDone);

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load the weapon prefab: " + name);
                yield break;
            }

            weaponPrefab = handle.Result;
            m_PrefabDict[name] = weaponPrefab;  //�����ص�Ԥ�Ƽ�����ֵ�
        }


        GameObject weaponObject = GameObject.Instantiate(weaponPrefab, isPrimary? PrimaryWeapon : SecondaryWeapon, false);

        //������Ԥ�Ƽ���ֵ��Player�ű��е���/������
        Player player = GameObject.FindObjectOfType<Player>();
        player.SetWeapon(weaponObject.GetComponent<Weapon>(), isPrimary);
    }


    //��Addressables���ͷ�������ֻ�����������ͷ��ڴ�
    public void ReleaseWeapon(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
            key = key.Substring(0, key.Length - 7);
        }


        if (m_PrefabDict.TryGetValue(key, out GameObject weaponPrefab))
        {
            Addressables.Release(weaponPrefab);

            //��Ԥ�Ƽ������ֵ����Ƴ���������
            m_PrefabDict.Remove(key);

            Debug.Log("Weapon released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This weapon is not loaded yet, cannot release: " + key);
        }
    }
}