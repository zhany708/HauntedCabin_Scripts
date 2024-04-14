using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    public SO_WeaponKeys WeaponKeys;
    public GameObject WeaponPreFab;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //������ײ������ҵ�combat�����壬���Ҫ��InParent

            //����ǰ�ȼ��WeaponKeys���Ƿ���Ҫʰȡ��װ��������
            if (WeaponKeys.ContainsKey(WeaponPreFab.name))
            {
                player.ChangeWeapon(WeaponPreFab.name, true);
                //Debug.Log("You got new weapon!");
            }

            else
            {
                Debug.LogError("Weapon key not recognized: " + WeaponPreFab.name);
            }


            //��Ҫʵ�֣�ͨ��һ�ַ�ʽ�����������������Ǹ�����

            Destroy(gameObject);    //��ɫ��������ٵ��ϵ�����
        }
    }
}