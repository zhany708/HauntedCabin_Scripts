using System.Collections;
using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    public SO_WeaponKeys WeaponKeys;
    public GameObject WeaponPreFab;


    PickupWeaponPanel weaponPickupPanel;








    private void Start()
    {
        //��ǰ����ʰȡ������UI����ֹ����
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



    //�����ײ���ϵ���������ʾ����ʰȡUI
    private IEnumerator ProcessWeaponPickup(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //������ײ������ҵ�combat�����壬���Ҫ��InParent

            yield return UIManager.Instance.OpenPanel(WeaponKeys.UIKeys.PickupWeaponPanelKey);

            
            weaponPickupPanel = FindAnyObjectByType<PickupWeaponPanel>();
            if (weaponPickupPanel != null)
            {
                //���������ָ�ֵ��UI��������ʾ
                weaponPickupPanel.SetItemName(WeaponPreFab.name);

                //����ɫ�ű����������崫�ݸ�UI�����ڲ�ͬ�İ�ť����
                weaponPickupPanel.SetPlayerAndWeapon(player, WeaponPreFab, this);
            }
        }
    }
}