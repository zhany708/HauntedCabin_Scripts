using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    public GameObject WeaponPreFab;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //由于碰撞的是玩家的combat子物体，因此要用InParent

            player.ChangeWeapon(WeaponPreFab, true);
            //需要实现：通过一种方式决定更换主武器还是副武器


            //Debug.Log("You got new weapon!");

            Destroy(gameObject);    //角色捡起后销毁武器
        }
    }
}
