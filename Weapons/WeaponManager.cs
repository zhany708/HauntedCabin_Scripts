using System.Threading.Tasks;
using UnityEngine;



public class WeaponManager : ManagerTemplate<WeaponManager>        //���ڹ��������ļ���/���ٵ�
{
    private Transform m_PrimaryWeapon;
    private Transform m_SecondaryWeapon;




    //Dictionary<string, Weapon> m_WeaponDict;      //�������ʹ�õ��������ֵ�






    protected override void Awake()
    {
        base.Awake();

        //��ֵ�������͸��������ű�
        SetupWeaponHolder(ref m_PrimaryWeapon, "PrimaryWeapon");
        SetupWeaponHolder(ref m_SecondaryWeapon, "SecondaryWeapon");
    }





    //���ýű�����������������
    private void SetupWeaponHolder(ref Transform weaponHolder, string weaponHolderName)
    {
        //����Ѱ����Ϸ�����ڵڶ������������ֵ����壬���û�ҵ��Ļ����½�һ��
        GameObject weaponHolderObject = GameObject.Find(weaponHolderName) ?? new GameObject(weaponHolderName);


        //�����Ѱ��Player�ű������������Ϊ�����ĸ�����
        Player player = FindObjectOfType<Player>();
        if (player)
        {
            weaponHolderObject.transform.SetParent(player.transform);
        }
        

        weaponHolder = weaponHolderObject.transform;
    }




    public async Task LoadWeapon(string weaponName, bool isPrimary)
    {
        //�첽���أ�������Ƿ���سɹ�
        GameObject weaponPrefab = await LoadPrefabAsync(weaponName);
        if (weaponPrefab == null)
        {
            Debug.LogError("Failed to load weapon prefab: " + name);
            return;
        }


        //�������壬�����ݵڶ�����������������Ϊ���������Ǹ�����
        GameObject weaponObject = Instantiate(weaponPrefab, isPrimary ? m_PrimaryWeapon : m_SecondaryWeapon);

        //��������������󣬴��ݸ���ҽű�
        EquipWeaponToPlayer(weaponObject, isPrimary);
    }




    //��������Ԥ�Ƽ���ֵ��Player�ű��е���/������
    private void EquipWeaponToPlayer(GameObject weaponObject, bool isPrimary)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (player)
        {
            player.SetWeapon(weaponObject.GetComponent<Weapon>(), isPrimary);
        }
    }
}