using Lean.Localization;
using System.Collections;
using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    //public SO_WeaponKeys WeaponKeys;
    public GameObject WeaponPreFab;

    public string WeaponPhraseKey;      //��Lean Localization�ǵ���ʱ��Ҫ��string





    PickupWeaponPanel weaponPickupPanel;

    bool m_IsPanelOpen = false;     //���ڷ�ֹ�����UI��ʾʱ���´���ʰȡ�����Ĵ��������µ��ظ���UI







    private void Awake()
    {
        if (WeaponPreFab == null)
        {
            Debug.LogError("Weapon prefab is not assigned in the WeaponPickUp script.");
            return;
        }

        if (WeaponPhraseKey == "")
        {
            Debug.LogError("WeaponPhraseKey is not written in the WeaponPickUp script.");
            return;
        }
    }

    private async void Start()
    {
        //��ǰ����ʰȡ������UI����ֹ����
        if (!string.IsNullOrEmpty(UIManager.Instance.UIKeys.PickupWeaponPanelKey))
        {
            await UIManager.Instance.InitPanel(UIManager.Instance.UIKeys.PickupWeaponPanelKey);
        }

        else
        {
            Debug.LogError("UIKeys not set or PickupWeaponPanelKey is empty.");
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        ProcessWeaponPickup(other);
    }






    //�����ײ���ϵ���������ʾ����ʰȡUI
    private async void ProcessWeaponPickup(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !m_IsPanelOpen)
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //������ײ������ҵ�combat�����壬���Ҫ��InParent

            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.PickupWeaponPanelKey);

            
            weaponPickupPanel = FindAnyObjectByType<PickupWeaponPanel>();
            if (weaponPickupPanel != null)
            {
                //���������ָ�ֵ��UI��������ʾ
                SetLocalizedText(WeaponPhraseKey);

                //����ɫ�ű����������崫�ݸ�UI�����ڲ�ͬ�İ�ť����
                weaponPickupPanel.SetPlayerAndWeapon(player, WeaponPreFab, this);

                //�ڴ��ݴ˽ű���UI�������ò���ֵ
                m_IsPanelOpen = true;
            }
        }
    }


    private void SetLocalizedText(string phraseKey)
    {
        if (LeanLocalization.CurrentLanguages != null)
        {
            //���ݵ�ǰ���Ը�ֵ�ı���ʰȡ��������
            weaponPickupPanel.SetItemName(LeanLocalization.GetTranslationText(phraseKey) );     
        }
    }

    #region Setters
    public void SetIsPanelOpen(bool isOpen)
    {
        m_IsPanelOpen = isOpen;
    }
    #endregion
}