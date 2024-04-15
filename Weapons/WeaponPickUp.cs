using System.Collections;
using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    public SO_WeaponKeys WeaponKeys;
    public GameObject WeaponPreFab;


    PickupWeaponPanel weaponPickupPanel;








    private void Start()
    {
        //提前加载拾取武器的UI。防止卡顿
        if (WeaponKeys != null && !string.IsNullOrEmpty(WeaponKeys.UIKeys.PickupWeaponPanelKey))
        {
            StartCoroutine(UIManager.Instance.InitPanel(WeaponKeys.UIKeys.PickupWeaponPanelKey));
        }

        else
        {
            Debug.LogError("UIKeys not set or PickupWeaponPanelKey is empty.");
        }
    }





    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(ProcessWeaponPickup(other));
    }



    //玩家碰撞地上的武器后，显示武器拾取UI
    private IEnumerator ProcessWeaponPickup(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //由于碰撞的是玩家的combat子物体，因此要用InParent

            yield return UIManager.Instance.OpenPanel(WeaponKeys.UIKeys.PickupWeaponPanelKey);

            
            weaponPickupPanel = FindAnyObjectByType<PickupWeaponPanel>();
            if (weaponPickupPanel != null)
            {
                //将武器名字赋值给UI，用于显示
                weaponPickupPanel.SetItemName(WeaponPreFab.name);

                //将角色脚本和武器物体传递给UI，用于不同的按钮功能
                weaponPickupPanel.SetPlayerAndWeapon(player, WeaponPreFab, this);
            }
        }
    }
}