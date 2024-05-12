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

    //�ĸ����Ե�ֵ
    public static float StrengthValue {  get; private set; }
    public static float SpeedValue { get; private set; }
    public static float SanityValue { get; private set; }
    public static float KnowledgeValue { get; private set; }


    HealthBar m_PlayerHealthBar;
    Player m_Player;






    protected override void Awake()
    {
        base.Awake();

        InitializePlayerStatus();
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
    private void InitializePlayerStatus()
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


        //��PlayerData�����ȡ����ֵ
        StrengthValue = m_Player.PlayerData.Strength;
        SpeedValue = m_Player.PlayerData.Speed;
        SanityValue = m_Player.PlayerData.Sanity;
        KnowledgeValue = m_Player.PlayerData.Knowledge;

        UpdateStatusUI();
    }



    public void ChangePropertyValue(PlayerProperty property, float changeValue)
    {
        switch (property)
        {
            case PlayerProperty.Strength:
                StrengthValue += changeValue;
                UpdateStatusUI();

                break;

            case PlayerProperty.Speed:
                SpeedValue += changeValue;
                UpdateStatusUI();

                break;

            case PlayerProperty.Sanity:
                SanityValue += changeValue;
                UpdateStatusUI();

                break;

            case PlayerProperty.Knowledge:
                KnowledgeValue += changeValue;
                UpdateStatusUI();

                break;

            default:
                Debug.Log("The parameter is not one of the PlayerProperty.");
                break;
        }
    }



    //������ҵ�����UI
    public void UpdateStatusUI()
    {
        if (m_Player != null)
        {
            //ͨ�����ַ�ʽ�����ڽű�������ı�ĳ���ֵ���ɫ������ǰ������ű�ʾҪ�ı����ɫ����������ű�ʾ��θı䵽��Ϊֹ��
            StrengthText.text = $"Strength: <color=#FF6B6B>{StrengthValue} </color>";
            SpeedText.text = $"Speed: <color=#FF6B6B>{SpeedValue} </color>";
            SanityText.text = $"Sanity: <color=#3D88FF>{SanityValue} </color>";
            KnowledgeText.text = $"Knowledge: <color=#3D88FF>{KnowledgeValue} </color>";
        }
    }
}



//����������Ե�ö��
public enum PlayerProperty
{
    Strength,
    Speed,
    Sanity,
    Knowledge
}