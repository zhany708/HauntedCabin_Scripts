using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PickupWeaponPanel : PanelWithButton
{
    public Button EquipOnPrimary;
    public Button EquipOnSecondary;
    public Button Leave;

    
    TextMeshProUGUI m_ItemNameText;     //武器名   
    GameObject m_WeaponPrefab;          //要拾取的武器物体
   
    Player m_Player;                    //玩家脚本   
    WeaponPickUp m_WeaponPickup;        //拾取武器脚本


    //用于点击按钮函数的枚举
    private enum ButtonAction
    {
        EquipOnPrimary,
        EquipOnSecondary,
        Leave
    }





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
        EquipOnPrimary.onClick.AddListener( () => OnButtonClicked(ButtonAction.EquipOnPrimary) );
        EquipOnSecondary.onClick.AddListener(() => OnButtonClicked(ButtonAction.EquipOnSecondary) );
        Leave.onClick.AddListener(() => OnButtonClicked(ButtonAction.Leave) );       
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        //界面完全淡出后调用此函数
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

       
        if (m_WeaponPickup != null)
        {
            //设置拾取武器脚本里的布尔
            m_WeaponPickup.SetIsPanelOpen(false);
        }
    }




    private void DestroyWeaponGameObject()
    {
        if (m_WeaponPickup != null)
        {
            //删除地上的武器
            Destroy(m_WeaponPickup.gameObject);
        }
    }


    



    private void OnButtonClicked(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.EquipOnPrimary:
                EquipWeapon(true);
                break;

            case ButtonAction.EquipOnSecondary:
                EquipWeapon(false);
                break;

            case ButtonAction.Leave:
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
    }



    private void EquipWeapon(bool isPrimary)
    {
        if (m_Player != null && m_WeaponPrefab != null)
        {
            m_Player.ChangeWeapon(m_WeaponPrefab.name, isPrimary);

            //装备完武器后淡出界面
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);

            //装备完武器后删除地上的武器物体
            DestroyWeaponGameObject();
        }
    }

    #region Setters
    public void SetItemName(string itemName)
    {
        if (m_ItemNameText != null && itemName != null)
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