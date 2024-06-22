using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PlayerStatusBar : BasePanel
{
    public static PlayerStatusBar Instance { get; private set; }

    //传递给HealthBar脚本的照片
    public Image HpImage;
    public Image HpEffectImage;    

    //四个属性相关的UI
    public TextMeshProUGUI StrengthText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI SanityText;
    public TextMeshProUGUI KnowledgeText;


    public Player Player      //Lazy load
    {
        get
        {
            if (m_Player == null)
            {
                m_Player = FindAnyObjectByType<Player>();
            }
            return m_Player;
        }
        private set { }
    }
    private Player m_Player;

    //需要做的：删除这里的static，随后更改外部函数调用这些变量的地方
    //四个属性的值
    public static float StrengthValue { get; private set; }
    public static float SpeedValue { get; private set; }
    public static float SanityValue { get; private set; }
    public static float KnowledgeValue { get; private set; }

    //四个属性对应的翻译文本的string
    public string StrengthPhraseKey;
    public string SpeedPhraseKey;
    public string SanityPhraseKey;
    public string KnowledgePhraseKey;



    HealthBar m_PlayerHealthBar;





    #region Unity内部函数
    protected override void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        CheckComponents();              //检查公有组件是否都存在
        InitializePlayerStatus();       //初始化
    }   

    private void Start()
    {
        //设置当前界面的名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.PlayerStatusBarKey;
        }

        UpdateStatusUI();       //进入游戏前更新显示的数值
    }

    private void OnEnable() 
    {
        UIManager.Instance.ImportantPanel.Add(this);    //将该界面加进列表，以在重置游戏时不被删除
    }
    #endregion



    //初始化血条相关的部分
    public void InitializePlayerStatus()
    {
        //从PlayerData哪里获取属性值
        StrengthValue = Player.PlayerData.Strength;
        SpeedValue = Player.PlayerData.Speed;
        SanityValue = Player.PlayerData.Sanity;
        KnowledgeValue = Player.PlayerData.Knowledge;

        SetImagesToHealthBar();            
    }

    public void SetImagesToHealthBar()      //用于将照片组件传递给玩家血条
    {
        //获取玩家血条的脚本组件
        m_PlayerHealthBar = Player.GetComponentInChildren<HealthBar>();
        if (m_PlayerHealthBar == null)
        {
            Debug.LogError("HealthBar component not found under Player object.");
            return;
        }

        //传递生命值图片
        m_PlayerHealthBar.SetHpImage(HpImage);
        m_PlayerHealthBar.SetHpEffectImage(HpEffectImage);
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



    //更新玩家的属性UI（用于正确的显示数值）
    public void UpdateStatusUI()
    {
        //获取翻译的文本组件
        string strengthFormat = LeanLocalization.GetTranslationText(StrengthPhraseKey);
        string speedFormat = LeanLocalization.GetTranslationText(SpeedPhraseKey);
        string sanityFormat = LeanLocalization.GetTranslationText(SanityPhraseKey);
        string knowledgeFormat = LeanLocalization.GetTranslationText(KnowledgePhraseKey);

        //赋值所有属性的数值
        StrengthText.text = string.Format(strengthFormat, StrengthValue);
        SpeedText.text = string.Format(speedFormat, SpeedValue);
        SanityText.text = string.Format(sanityFormat, SanityValue);
        KnowledgeText.text = string.Format(knowledgeFormat, KnowledgeValue);       
    }



    private void CheckComponents()
    {
        //检查四个属性组件是否有的为空
        if (StrengthText == null || SpeedText == null || SanityText == null || KnowledgeText == null)
        {
            Debug.LogError("Some Text components are not assigned in the " + name);
            return;
        }

        if (StrengthPhraseKey == "" || SpeedPhraseKey == "" || SanityPhraseKey == "" || KnowledgePhraseKey == "")
        {
            Debug.LogError("Some Lean Localization phrase keys are not written in the " + name);
            return;
        }
    }


    //需要做的：删除这里的static，随后更改所有调用这两个函数的地方
    public static float GetStrengthAddition()   //每当玩家造成伤害时都需要调用此函数
    {
        return 1 + StrengthValue * 0.05f;       //每一点力量对应5%的伤害加成
    }

    public static float GetSpeedAddition()   //每当玩家移动时都需要调用此函数
    {
        return 1 + SpeedValue * 0.05f;       //每一点速度对应5%的移速加成
    }



    public void ResetGame()      //重置游戏
    {
        //重新赋予玩家的所有属性
        StrengthValue = Player.PlayerData.Strength;
        SpeedValue = Player.PlayerData.Speed;
        SanityValue = Player.PlayerData.Sanity;
        KnowledgeValue = Player.PlayerData.Knowledge;

        UpdateStatusUI();
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