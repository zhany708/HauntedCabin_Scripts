using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PlayerStatusBar : BasePanel
{
    public Image HpImage;
    public Image HpEffectImage;     //Ѫ������ͼƬ

    //�ĸ�������ص�UI
    public TextMeshProUGUI StrengthText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI SanityText;
    public TextMeshProUGUI KnowledgeText;

    HealthBar m_PlayerHealthBar;
    Player m_Player;




    protected override void Awake()
    {
        base.Awake();

        InitializeHealthBar();
    }

    private void Start()
    {
        //����ĸ���������Ƿ��е�Ϊ��
        if (StrengthText == null || SpeedText == null || SanityText == null || KnowledgeText == null)
        {
            Debug.LogError("Some Status Text is not assigned.");
            return;
        }
    }



    //��ʼ��Ѫ����صĲ���
    private void InitializeHealthBar()
    {
        //��ȡPlayerHealthBar��Ϸ����
        GameObject playerHealthBarObject = GameObject.Find("PlayerHealthBar");

        if (playerHealthBarObject == null)
        {
            Debug.LogError("PlayerHealthBar object not found.");
            return;
        }

        //��ȡ�ű����
        m_PlayerHealthBar = playerHealthBarObject.GetComponent<HealthBar>();
        if (m_PlayerHealthBar == null)
        {
            Debug.LogError("HealthBar component not found on PlayerHealthBar.");
            return;
        }

        //��������ֵͼƬ
        m_PlayerHealthBar.SetHpImage(HpImage);
        m_PlayerHealthBar.SetHpEffectImage(HpEffectImage);


        m_Player = FindObjectOfType<Player>();
        if (m_Player == null)
        {
            Debug.LogError("Player component not found.");
            return;
        }

        UpdateStatusUI();
    }


    //������ҵ�����UI
    public void UpdateStatusUI()
    {
        if (m_Player != null)
        {
            //ͨ�����ַ�ʽ�����ڽű�������ı�ĳ���ֵ���ɫ������ǰ������ű�ʾҪ�ı����ɫ����������ű�ʾ��θı䵽��Ϊֹ��
            StrengthText.text = $"Strength: <color=#FF6B6B>{m_Player.PlayerData.Strength} </color>";
            SpeedText.text = $"Speed: <color=#FF6B6B>{m_Player.PlayerData.Speed} </color>";
            SanityText.text = $"Sanity: <color=#3D88FF>{m_Player.PlayerData.Sanity} </color>";
            KnowledgeText.text = $"Knowledge: <color=#3D88FF>{m_Player.PlayerData.Knowledge} </color>";
        }
    }
}