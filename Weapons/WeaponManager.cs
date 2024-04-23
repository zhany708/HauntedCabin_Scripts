using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class WeaponManager : MonoBehaviour        //���ڹ��������ļ���/���ٵ�
{
    public static WeaponManager Instance { get; private set; }


    private Transform m_PrimaryWeapon;
    private Transform m_SecondaryWeapon;




    //Dictionary<string, Weapon> m_WeaponDict;      //�������ʹ�õ��������ֵ�
    Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();     //Ԥ�Ƽ������ֵ�






    void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

        //��ֵ�������͸��������ű�
        SetupWeaponHolder(ref m_PrimaryWeapon, "PrimaryWeapon");
        SetupWeaponHolder(ref m_SecondaryWeapon, "SecondaryWeapon");
    }





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
        GameObject weaponPrefab = await LoadWeaponAsync(weaponName);
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







    //�첽����
    private async Task<GameObject> LoadWeaponAsync(string name)
    {
        //����ֵ����Ƿ�������������еĻ�ֱ�ӷ���
        if (!m_PrefabDict.TryGetValue(name, out GameObject weaponPrefab))
        {
            //�첽��������
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            await handle.Task;

            //����첽�����Ƿ�ɹ�
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                weaponPrefab = handle.Result;

                //������Ԥ�Ƽ�����ֵ�
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


    //��������Ԥ�Ƽ���ֵ��Player�ű��е���/������
    private void EquipWeaponToPlayer(GameObject weaponObject, bool isPrimary)
    {
        Player player = GameObject.FindObjectOfType<Player>();

        if (player)
        {
            player.SetWeapon(weaponObject.GetComponent<Weapon>(), isPrimary);
        }
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