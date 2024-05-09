using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PickupWeaponPanel : PanelWithButton
{
    public Button EquipOnPrimary;
    public Button EquipOnSecondary;
    public Button Leave;

    
    TextMeshProUGUI m_ItemNameText;     //������   
    GameObject m_WeaponPrefab;          //Ҫʰȡ����������
   
    Player m_Player;                    //��ҽű�   
    WeaponPickUp m_WeaponPickup;        //ʰȡ�����ű�


    //���ڵ����ť������ö��
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
        if (m_ItemNameText == null)
        {
            Debug.LogError("ItemName text is not assigned in the PickupWeaponPanel.");
            return;
        }


        if (EquipOnPrimary == null || EquipOnSecondary == null || Leave == null)
        {
            Debug.LogError("Some buttons are not assigned in the PickupWeaponPanel.");
            return;
        }


        //Ĭ�ϰ�ťΪ��װ��������������ť���������Awake�����У�
        firstSelectedButton = EquipOnPrimary.gameObject;

        //���ô˽���ĵ���ֵ���������Awake�����У�
        FadeInAlpha = 0.75f;
    }

    private void Start()
    {
        //����ť�ͺ���������
        EquipOnPrimary.onClick.AddListener(() => OnButtonClicked(ButtonAction.EquipOnPrimary));
        EquipOnSecondary.onClick.AddListener(() => OnButtonClicked(ButtonAction.EquipOnSecondary));
        Leave.onClick.AddListener(() => OnButtonClicked(ButtonAction.Leave));  
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        //������ȫ��������ô˺���
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

       
        if (m_WeaponPickup != null)
        {
            //����ʰȡ�����ű���Ĳ���
            m_WeaponPickup.SetIsPanelOpen(false);
        }
    }




    private void DestroyWeaponGameObject()
    {
        if (m_WeaponPickup != null)
        {
            //ɾ�����ϵ�����
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

            //װ���������󵭳�����
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);

            //װ����������ɾ�����ϵ���������
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
        //������Щ�ű�����װ�����������ٵ��ϵ�����
        m_Player = thisPlayer;
        m_WeaponPrefab = thisWeapon;
        m_WeaponPickup = thisWeaponPickup;
    }
    #endregion
}