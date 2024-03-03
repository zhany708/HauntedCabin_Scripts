using System.Collections.Generic;
using UnityEngine;



public class WeaponConst    //���ڴ洢����������
{
    public const string Dagger = "Dagger";    //���״̬������HealthBar�ű����ʼ��
    public const string Shotgun = "Shotgun";    //������׶�����
}




public class PlayerInventory : MonoBehaviour        //���ڴ�������
{
    public GameObject[] PrimaryWeapon;
    public GameObject[] SecondaryWeapon;


    Dictionary<string, Weapon> m_WeaponDict;      //�������ʹ�õ��������ֵ�
    Dictionary<string, string> m_PathDict;       //ʹ���ֵ�洢����������·����
    Dictionary<string, GameObject> m_PrefabDict;     //Ԥ�Ƽ������ֵ�




    private void Awake()
    {
        InitDicts();
    }



    private void InitDicts()
    {
        m_WeaponDict = new Dictionary<string, Weapon>();
        m_PrefabDict = new Dictionary<string, GameObject>();

        m_PathDict = new Dictionary<string, string>()   //��ʼ�����н����·��
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

