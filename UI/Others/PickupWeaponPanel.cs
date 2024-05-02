using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PickupWeaponPanel : PanelWithButton
{
    public Button EquipOnPrimary;
    public Button EquipOnSecondary;
    public Button Leave;

    //������
    TextMeshProUGUI m_ItemNameText;
    //Ҫʰȡ����������
    GameObject m_WeaponPrefab;

    //��ҽű�
    Player m_Player;
    //ʰȡ�����ű�
    WeaponPickUp m_WeaponPickup;



    protected override void Awake()
    {
        base.Awake();

        m_ItemNameText = GetComponentInChildren<TextMeshProUGUI>();

        //Ĭ�ϰ�ťΪ��װ��������������ť
        firstSelectedButton = EquipOnPrimary.gameObject;       
    }

    private void Start()
    {
        if (EquipOnPrimary == null || EquipOnSecondary == null || Leave == null) 
        {
            Debug.LogError("Some buttons are not assigned in the PickupWeaponPanel.");
            return;
        }

        //����ť�ͺ���������
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

        //�����ʱ��ֹ��ҹ���
        playerInputHandler.SetCanDetectAttack(false);
    }

    public override void ClosePanel()
    {
        base.ClosePanel();

        //����رպ�������ҹ���
        playerInputHandler.SetCanDetectAttack(true);

        m_WeaponPickup.SetIsPanelOpen(false);
    }



    private void ButtonAction(string action)
    {
        switch (action)
        {
            case "EquipOnPrimary":
                //װ��������������
                m_Player.ChangeWeapon(m_WeaponPrefab.name, true);

                //װ�����رս��棬��ɾ�����ϵ�����
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                Destroy(m_WeaponPickup.gameObject);
                break;

            case "EquipOnSecondary":
                //װ��������������
                m_Player.ChangeWeapon(m_WeaponPrefab.name, false);

                //װ�����رս��棬��ɾ�����ϵ�����
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
        //������Щ�ű�����װ�����������ٵ��ϵ�����
        m_Player = thisPlayer;
        m_WeaponPrefab = thisWeapon;
        m_WeaponPickup = thisWeaponPickup;
    }
    #endregion
}