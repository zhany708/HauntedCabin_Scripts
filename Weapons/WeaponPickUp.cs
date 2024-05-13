using Lean.Localization;
using System.Collections;
using UnityEngine;


public class WeaponPickUp : MonoBehaviour
{
    //public SO_WeaponKeys WeaponKeys;
    public GameObject WeaponPreFab;

    public string WeaponPhraseKey;      //从Lean Localization那调用时需要的string





    PickupWeaponPanel weaponPickupPanel;

    bool m_IsPanelOpen = false;     //用于防止玩家在UI显示时重新触发拾取武器的触发器导致的重复打开UI







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
        //提前加载拾取武器的UI。防止卡顿
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






    //玩家碰撞地上的武器后，显示武器拾取UI
    private async void ProcessWeaponPickup(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !m_IsPanelOpen)
        {
            Player player = other.gameObject.GetComponentInParent<Player>();    //由于碰撞的是玩家的combat子物体，因此要用InParent

            await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.PickupWeaponPanelKey);

            
            weaponPickupPanel = FindAnyObjectByType<PickupWeaponPanel>();
            if (weaponPickupPanel != null)
            {
                //将武器名字赋值给UI，用于显示
                SetLocalizedText(WeaponPhraseKey);

                //将角色脚本和武器物体传递给UI，用于不同的按钮功能
                weaponPickupPanel.SetPlayerAndWeapon(player, WeaponPreFab, this);

                //在传递此脚本给UI后，再设置布尔值
                m_IsPanelOpen = true;
            }
        }
    }


    private void SetLocalizedText(string phraseKey)
    {
        if (LeanLocalization.CurrentLanguages != null)
        {
            //根据当前语言赋值文本给拾取武器界面
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