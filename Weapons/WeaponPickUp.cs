using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    public SO_WeaponKeys WeaponKeys;
    public GameObject WeaponPreFab;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //由于碰撞的是玩家的combat子物体，因此要用InParent

            //加载前先检查WeaponKeys中是否有要拾取的装备的名字
            if (WeaponKeys.ContainsKey(WeaponPreFab.name))
            {
                player.ChangeWeapon(WeaponPreFab.name, true);
                //Debug.Log("You got new weapon!");
            }

            else
            {
                Debug.LogError("Weapon key not recognized: " + WeaponPreFab.name);
            }


            //需要实现：通过一种方式决定更换主武器还是副武器

            Destroy(gameObject);    //角色捡起后销毁地上的武器
        }
    }
}