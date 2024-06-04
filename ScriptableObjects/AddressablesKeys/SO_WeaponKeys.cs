using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newWeaponKeys", menuName = "Data/Weapon Data/Weapon Keys")]
public class SO_WeaponKeys : ScriptableObject
{
    //在编辑器里将所有名字加进列表
    public List<string> weaponKeyList = new List<string>();
    private HashSet<string> weaponKeys;     //哈希表的搜寻速度为O(1)




    private void OnEnable()
    {
        //在列表中填充哈希表以快速访问其中的元素
        weaponKeys = new HashSet<string>(weaponKeyList);
    }

    public bool ContainsKey(string key)
    {
        return weaponKeys.Contains(key);
    }
}