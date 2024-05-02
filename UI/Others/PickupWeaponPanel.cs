using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PickupWeaponPanel : PanelWithButton
{
    public Button EquipOnPrimary;
    public Button EquipOnSecondary;
    public Button Leave;

    //武器名
    TextMeshProUGUI m_ItemNameText;
    //要拾取的武器物体
    GameObject m_WeaponPrefab;

    //玩家脚本
    Player m_Player;
    //拾取武器脚本
    WeaponPickUp m_WeaponPickup;



    protected override void Awake()
    {
        base.Awake();

        m_ItemNameText = GetComponentInChildren<TextMeshProUGUI>();

        //默认按钮为“装备在主武器”按钮
        firstSelectedButton = EquipOnPrimary.gameObject;       
    }

    private void Start()
    {
        if (EquipOnPrimary == null || EquipOnSecondary == null || Leave == null) 
        {
            Debug.LogError("Some buttons are not assigned in the PickupWeaponPanel.");
            return;
        }

        //将按钮和函数绑定起来
        EquipOnPrimary.onClick.AddListener( () => ButtonAction(EquipOnPrimary.name) );
        EquipOnSecondary.onClick.AddListener(() => ButtonAction(EquipOnSecondary.name));
        Leave.onClick.AddListener(() => ButtonAction(Leave.name));       
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        OnFadeOutFinished += ClosePanel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        OnFadeOutFinished -= ClosePanel;
    }





    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        //界面打开时禁止玩家攻击
        playerInputHandler.SetCanDetectAttack(false);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        //界面关闭后允许玩家攻击
        playerInputHandler.SetCanDetectAttack(true);

        m_WeaponPickup.SetIsPanelOpen(false);
    }



    private void ButtonAction(string action)
    {
        switch (action)
        {
            case "EquipOnPrimary":
                //装备武器到主武器
                m_Player.ChangeWeapon(m_WeaponPrefab.name, true);

                //装备完后关闭界面，并删除地上的武器
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                Destroy(m_WeaponPickup.gameObject);
                break;

            case "EquipOnSecondary":
                //装备武器到副武器
                m_Player.ChangeWeapon(m_WeaponPrefab.name, false);

                //装备完后关闭界面，并删除地上的武器
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                Destroy(m_WeaponPickup.gameObject);
                break;

            case "Leave":
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
    }


    #region Setters
    public void SetItemName(string itemName)
    {
        if (itemName != null)
        {
            m_ItemNameText.text = itemName;
        }
    }

    public void SetPlayerAndWeapon(Player thisPlayer, GameObject thisWeapon, WeaponPickUp thisWeaponPickup)
    {
        //传递这些脚本用于装备武器和销毁地上的武器
        m_Player = thisPlayer;
        m_WeaponPrefab = thisWeapon;
        m_WeaponPickup = thisWeaponPickup;
    }
    #endregion
}