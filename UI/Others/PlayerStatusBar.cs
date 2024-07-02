using Lean.Localization;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class PlayerStatusBar : BasePanel
{
    public static PlayerStatusBar Instance { get; private set; }

    //传递给HealthBar脚本的照片
    public Image HpImage;
    public Image IncreaseHpEffectImage;
    public Image DecreaseHpEffectImage;    

    //四个属性相关的UI
    public TextMeshProUGUI StrengthText;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI SanityText;
    public TextMeshProUGUI KnowledgeText;

    //玩家的血条的文本
    //public TextMeshProUGUI HealthText;



    //四个属性对应的翻译文本的string
    public string StrengthPhraseKey;
    public string SpeedPhraseKey;
    public string SanityPhraseKey;
    public string KnowledgePhraseKey;

    //玩家的血条对应的翻译文本的string
    //public string HealthPhraseKey;



    public Player Player      //Lazy load（只在需要变量时才尝试获取组件，而不是一次性的放在某个Unity内部函数中）
    {
        get
        {
            if (m_Player == null)
            {
                m_Player = FindAnyObjectByType<Player>();
                //如果尝试获取组件后Player变量仍然为空的话，则报错
                if (m_Player == null)
                {
                    Debug.Log("Cannot get the reference of the Player component in the " + name);
                }
            }
            return m_Player;
        }
        private set { }
    }
    private Player m_Player;


    //四个属性的值
    public float StrengthValue { get; private set; } = 0;
    public float SpeedValue { get; private set; } = 0;
    public float SanityValue { get; private set; } = 0;
    public float KnowledgeValue { get; private set; } = 0;

    



    PlayerHealthBar m_PlayerHealthBar;


    float m_CurrentHealth => m_PlayerHealthBar.GetCurrentHp();    //玩家的当前生命值
    float m_MaxHealth => m_PlayerHealthBar.GetMaxHp();            //玩家的最大生命值





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
    }   

    private void Start()
    {
        //设置当前界面的名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.PlayerStatusBarKey;
        }


        if (!UIManager.Instance.PanelDict.ContainsKey(panelName) && Instance == this)
        {
            //Debug.Log(panelName + " added into the PanelDict!");

            //将界面加进字典，表示界面已经打开
            UIManager.Instance.PanelDict.Add(panelName, this);
        }

        UIManager.Instance.ImportantPanel.Add(this);    //将该界面加进列表，以在重置游戏时不被删除
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDisable() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;


        //先检查界面名字是否为空（为空的话则代表当前界面是重复的，因为是在Start函数中赋值名字）
        if (panelName != null)
        {
            //只有当前界面是唯一时且被删除后，才运行以下逻辑
            if (UIManager.Instance.PanelDict.ContainsKey(panelName) && Instance == this)
            {
                //从字典中移除，表示界面没打开
                UIManager.Instance.PanelDict.Remove(panelName);
            }
        }
    }
    #endregion


    #region 初始化相关
    //初始化血条相关的部分
    public void InitializePlayerStatus()
    {
        if (Player != null)
        {
            ResetGame();

            //将照片组件传递给玩家血条
            SetImagesToHealthBar();
        }            
    }

    public void SetImagesToHealthBar()      //用于将照片组件传递给玩家血条
    {
        //获取玩家血条的脚本组件
        m_PlayerHealthBar = Player.GetComponentInChildren<PlayerHealthBar>();
        if (m_PlayerHealthBar == null)
        {
            Debug.LogError("HealthBar component not found under Player object.");
            return;
        }

        //传递血条和缓冲图片
        m_PlayerHealthBar.SetHpImage(HpImage);
        m_PlayerHealthBar.SetIncreaseHpEffectImage(IncreaseHpEffectImage);
        m_PlayerHealthBar.SetDecreaseHpEffectImage(DecreaseHpEffectImage);

        Debug.Log("SetImagesToHealthBar is called in the: " + name);
    }

    private void CheckComponents()
    {
        //检查四个属性和血条文本组件是否有的为空
        if (StrengthText == null || SpeedText == null || SanityText == null || KnowledgeText == null)// || HealthText == null)
        {
            Debug.LogError("Some Text components are not assigned in the " + name);
            return;
        }

        //检查各个翻译文本是否为空
        if (StrengthPhraseKey == "" || SpeedPhraseKey == "" || SanityPhraseKey == "" || KnowledgePhraseKey == "")// || HealthPhraseKey == "")
        {
            Debug.LogError("Some Lean Localization phrase keys are not written in the " + name);
            return;
        }
    }
    #endregion


    #region 更改属性相关
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

        //Debug.Log("The translation format are: " + "\n" + strengthFormat + "\n" + speedFormat + "\n" + sanityFormat + "\n" + knowledgeFormat);

        //赋值所有属性的数值
        StrengthText.text = string.Format(strengthFormat, StrengthValue);
        SpeedText.text = string.Format(speedFormat, SpeedValue);
        SanityText.text = string.Format(sanityFormat, SanityValue);
        KnowledgeText.text = string.Format(knowledgeFormat, KnowledgeValue);

        //Debug.Log("The four texts are: " + "\n" + StrengthText.text + "\n" + SpeedText.text + "\n" + SanityText.text + "\n" + KnowledgeText.text);
    }

    //用于防止因为换场景等原因导致重新加载后无法正常显示数值的情况
    public IEnumerator DelayedUpdateStatusUI()
    {
        yield return new WaitForEndOfFrame();       //等待一帧的结束，以便所有其余的所需内容都已初始化完成
        UpdateStatusUI();
    }



    /*
    //更新玩家的血条文本
    public void UpdateHealthText()
    {
        //获取翻译的文本组件
        string healthFormat = LeanLocalization.GetTranslationText(HealthPhraseKey);

        //赋值血条的数值
        HealthText.text = string.Format(healthFormat, m_CurrentHealth, m_MaxHealth);            
    }
    */
    #endregion


    #region 其余函数
    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //每当进入一楼场景时都调用以下逻辑
        if (scene.name == "FirstFloor")
        {
            if (Instance == this)
            {
                ResetGame();

                //设置界面的透明度（显示出来）
                CanvasGroup.alpha = FadeInAlpha;
            }
            
            //将更新血条文本的函数跟玩家血条脚本绑定起来
            //m_PlayerHealthBar.OnHealthChange += UpdateHealthText;
        }

        //进入其余场景时（目前只有主菜单）
        else
        {
            if (Instance == this)
            {
                //设置界面的透明度（隐藏界面）
                CanvasGroup.alpha = FadeOutAlpha;
            }
                
            //m_PlayerHealthBar.OnHealthChange -= UpdateHealthText;
        }
    }



    public float GetStrengthAddition()   //每当玩家造成伤害时都需要调用此函数
    {
        return 1 + StrengthValue * 0.05f;       //每一点力量对应5%的伤害加成
    }

    public float GetSpeedAddition()   //每当玩家移动时都需要调用此函数
    {
        return 1 + SpeedValue * 0.05f;       //每一点速度对应5%的移速加成
    }



    public void ResetGame()      //重置游戏
    {
        //重新赋予玩家的所有属性（如果在换场景前调用此函数，则Player引用仍然存在）
        if (Player != null)
        {
            //从PlayerData获取属性值
            StrengthValue = Player.PlayerData.Strength;
            SpeedValue = Player.PlayerData.Speed;
            SanityValue = Player.PlayerData.Sanity;
            KnowledgeValue = Player.PlayerData.Knowledge;

            UpdateStatusUI();       //赋值后将数值正确的显示出来


            //重置玩家的血量
            Stats playerStats = Player.GetComponentInChildren<Stats>();      //获取玩家血条的脚本组件
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats component not found under Player object.");
                return;
            }

            playerStats.SetCurrentHealth(playerStats.MaxHealth);             //重置玩家的血量（以及血条占比）
        }
    }
    #endregion
}



//用于玩家属性的枚举
public enum PlayerProperty
{
    Strength,
    Speed,
    Sanity,
    Knowledge
}