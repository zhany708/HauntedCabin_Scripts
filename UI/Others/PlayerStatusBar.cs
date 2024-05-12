using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PlayerStatusBar : BasePanel
{
    public Image HpImage;
    public Image HpEffectImage;     //血量缓冲图片

    //四个属性相关的UI
    public TextMeshProUGUI StrengthText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI SanityText;
    public TextMeshProUGUI KnowledgeText;

    //四个属性的值
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
        //检查四个属性组件是否有的为空
        if (StrengthText == null || SpeedText == null || SanityText == null || KnowledgeText == null)
        {
            Debug.LogError("Some Status Text is not assigned.");
            return;
        }
    }



    //初始化血条相关的部分
    private void InitializePlayerStatus()
    {
        //获取PlayerHealthBar游戏物体
        GameObject playerHealthBarObject = GameObject.Find("PlayerHealthBar");

        if (playerHealthBarObject == null)
        {
            Debug.LogError("PlayerHealthBar object not found.");
            return;
        }

        //获取脚本组件
        m_PlayerHealthBar = playerHealthBarObject.GetComponent<HealthBar>();
        if (m_PlayerHealthBar == null)
        {
            Debug.LogError("HealthBar component not found on PlayerHealthBar.");
            return;
        }

        //传递生命值图片
        m_PlayerHealthBar.SetHpImage(HpImage);
        m_PlayerHealthBar.SetHpEffectImage(HpEffectImage);


        m_Player = FindObjectOfType<Player>();
        if (m_Player == null)
        {
            Debug.LogError("Player component not found.");
            return;
        }


        //从PlayerData哪里获取属性值
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



    //更新玩家的属性UI
    public void UpdateStatusUI()
    {
        if (m_Player != null)
        {
            //通过这种方式可以在脚本里更改文本某部分的颜色（单词前面的括号表示要改变的颜色，后面的括号表示这次改变到此为止）
            StrengthText.text = $"Strength: <color=#FF6B6B>{StrengthValue} </color>";
            SpeedText.text = $"Speed: <color=#FF6B6B>{SpeedValue} </color>";
            SanityText.text = $"Sanity: <color=#3D88FF>{SanityValue} </color>";
            KnowledgeText.text = $"Knowledge: <color=#3D88FF>{KnowledgeValue} </color>";
        }
    }
}



//用于玩家属性的枚举
public enum PlayerProperty
{
    Strength,
    Speed,
    Sanity,
    Knowledge
}