using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newWeaponKeys", menuName = "Data/Weapon Data/Weapon Keys")]
public class SO_WeaponKeys : ScriptableObject
{
    public SO_UIKeys UIKeys;

    //�ڱ༭���ｫ�������ּӽ��б�
    public List<string> weaponKeyList = new List<string>();
    private HashSet<string> weaponKeys;     //��ϣ�����Ѱ�ٶ�ΪO(1)




    private void OnEnable()
    {
        //���б�������ϣ���Կ��ٷ������е�Ԫ��
        weaponKeys = new HashSet<string>(weaponKeyList);
    }

    public bool ContainsKey(string key)
    {
        return weaponKeys.Contains(key);
    }
}