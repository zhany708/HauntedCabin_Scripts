using System.Collections.Generic;
using UnityEngine;



public class WeaponConst    //���ڴ洢����������
{
    public const string Dagger = "Dagger";    //��ʼذ��
    public const string Shotgun = "Shotgun";    //����ǹ
}




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
    Dictionary<string, string> m_PathDict;       //ʹ���ֵ�洢����������·����
    Dictionary<string, GameObject> m_PrefabDict;     //Ԥ�Ƽ������ֵ�








    private WeaponInventory()
    {
        InitDicts();
    }


    private void InitDicts()
    {
        m_PrefabDict = new Dictionary<string, GameObject>();

        m_PathDict = new Dictionary<string, string>()   //��ʼ�����н����·��
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


        //��黺���Ԥ�Ƽ����Ƿ��Ѿ���Ҫ���ɵ��������������ֱ�ӻ�ȡ�������������ļ��
        if (m_PrefabDict.TryGetValue(name, out weaponPrefab))
        {
            if (isPrimary)
            {
                weaponObject = GameObject.Instantiate(weaponPrefab, PrimaryWeapon, false);    //���ɳ�������ʹ����Ϊ��������������
            }
            else
            {
                weaponObject = GameObject.Instantiate(weaponPrefab, SecondaryWeapon, false);    //���ɳ�������ʹ����Ϊ��������������
            }


            weapon = weaponObject.GetComponent<Weapon>();
            return weapon;
        }





        //��������Ƿ���·������
        string path = "";

        if (!m_PathDict.TryGetValue(name, out path))
        {
            Debug.LogError("Something wrong with the name or path of this panel: " + name);
            return null;
        }


        //ʹ�û����Ԥ�Ƽ�
        if (!m_PrefabDict.TryGetValue(name, out weaponPrefab))
        {
            string realPath = "Prefab/Weapons/" + path;    //���û�б����ع�������س��������뻺���ֵ�

            weaponPrefab = Resources.Load<GameObject>(realPath);     //ͨ��Load������Assets��Ѱ����Դ��ֵ��������Resources�ļ������棩
            m_PrefabDict.Add(name, weaponPrefab);    //����������Ԥ�Ƽ����ֵ�
        }


        if (isPrimary)
        {
            weaponObject = GameObject.Instantiate(weaponPrefab, PrimaryWeapon, false);    //���ɳ�������ʹ����Ϊ��������������
        }
        else
        {
            weaponObject = GameObject.Instantiate(weaponPrefab, SecondaryWeapon, false);    //���ɳ�������ʹ����Ϊ��������������
        }


        weapon = weaponObject.GetComponent<Weapon>();
        return weapon;
    }
}

