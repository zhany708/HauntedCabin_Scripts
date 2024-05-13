using Lean.Localization;
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

    //�ĸ����Զ�Ӧ�ķ����ı���string
    public string StrengthPhraseKey;
    public string SpeedPhraseKey;
    public string SanityPhraseKey;
    public string KnowledgePhraseKey;



    HealthBar m_PlayerHealthBar;
    Player m_Player;






    protected override void Awake()
    {
        base.Awake();

        CheckComponents();              //��鹫������Ƿ񶼴���
        InitializePlayerStatus();       //��ʼ��
    }   

    private void Start()
    {
        //����ĸ���������Ƿ��е�Ϊ��
        if (StrengthText == null || SpeedText == null || SanityText == null || KnowledgeText == null)
        {
            Debug.LogError("Some Status Text is not assigned.");
            return;
        }

        UpdateStatusUI();       //������Ϸǰ������ʾ����ֵ
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
            //��ȡ������ı����
            string strengthFormat = LeanLocalization.GetTranslationText(StrengthPhraseKey);
            string speedFormat = LeanLocalization.GetTranslationText(SpeedPhraseKey);
            string sanityFormat = LeanLocalization.GetTranslationText(SanityPhraseKey);
            string knowledgeFormat = LeanLocalization.GetTranslationText(KnowledgePhraseKey);

            //��ֵ�������Ե���ֵ
            StrengthText.text = string.Format(strengthFormat, StrengthValue);
            SpeedText.text = string.Format(speedFormat, SpeedValue);
            SanityText.text = string.Format(sanityFormat, SanityValue);
            KnowledgeText.text = string.Format(knowledgeFormat, KnowledgeValue);
        }
    }



    private void CheckComponents()
    {
        if (StrengthText == null || SpeedText == null || SanityText == null || KnowledgeText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the PlayerStatusBar.");
            return;
        }

        if (StrengthPhraseKey == "" || SpeedPhraseKey == "" || SanityPhraseKey == "" || KnowledgePhraseKey == "")
        {
            Debug.LogError("Some Lean Localization phrase keys are not written in the PlayerStatusBar.");
            return;
        }
    }



    public static float GetStrengthAddition()   //ÿ���������˺�ʱ����Ҫ���ô˺���
    {
        return 1 + StrengthValue * 0.05f;       //ÿһ��������Ӧ5%���˺��ӳ�
    }

    public static float GetSpeedAddition()   //ÿ������ƶ�ʱ����Ҫ���ô˺���
    {
        return 1 + SpeedValue * 0.05f;       //ÿһ���ٶȶ�Ӧ5%�����ټӳ�
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