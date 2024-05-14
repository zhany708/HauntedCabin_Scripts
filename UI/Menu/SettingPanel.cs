using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




//虽然该界面只有下拉列表，但是为了防止玩家打开界面后自由自动和攻击，所以继承自PanelWithButton
public class SettingPanel : PanelWithButton     
{
    public TMP_Dropdown Dropdown;       //下拉列表
    public TMP_Text LabelText;          //下拉列表中显示当前选项的文本
    public Button CloseButton;          //用于关闭界面的按钮

    LeanLocalization m_Localization;       //翻译处理器的引用







    protected override void Awake()
    {
        if (Dropdown == null || LabelText == null || CloseButton == null)
        {
            Debug.LogError("Some TMP components are not assigned in the SettingPanel.");
            return;
        }


        m_Localization = FindObjectOfType<LeanLocalization>();  //寻找翻译组件

        if (m_Localization == null)
        {
            Debug.LogError("LeanLocalization component is not assigned in the SettingPanel.");
            return;
        }
    }

    private void Start()
    {
        PopulateDropdown();     //根据拥有的语言填充下拉菜单

        Dropdown.onValueChanged.AddListener(ChangeLanguage);    //将函数绑定到下拉菜单
        CloseButton.onClick.AddListener(() => Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false) );   //将函数绑定到按钮
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







    private void PopulateDropdown()     //根据拥有的语言填充下拉菜单
    {
        Dropdown.ClearOptions();    //先清空所有原本的选项

        foreach (var languageDict in LeanLocalization.CurrentLanguages)
        {
            string language = languageDict.Key;     //根据value从字典那获取key         

            //转换语言后，创建新的字符串给下拉菜单
            Dropdown.options.Add(new TMP_Dropdown.OptionData(LanguageTransform(language)) );      
        }

        //转换语言后，设置当前语言对应的值
        Dropdown.value = Dropdown.options.FindIndex(option => option.text == LanguageTransform(m_Localization.CurrentLanguage) );
        //转换语言后，给Label赋值当前的语言，用于显示当前下拉菜单正在选择的值
        LabelText.text = LanguageTransform(m_Localization.CurrentLanguage);      
    }


    private void ChangeLanguage(int index)
    {
        string selectedLanguage = Dropdown.options[index].text;     //根据下拉菜单的值获取对应的文本的字符串

        //转换语言后，切换到字符串对应的语言
        m_Localization.SetCurrentLanguage(LanguageTransform(selectedLanguage) );
    }


    //让文本直接显示那个语言，从而让玩家可以更快识别；或者转换回来
    private string LanguageTransform(string selectedLanguage)
    {
        switch (selectedLanguage)
        {
            case "Chinese":
                selectedLanguage = "中文";
                break;

            case "中文":
                selectedLanguage = "Chinese";
                break;

            default:
                selectedLanguage = "English";       //默认返回English
                break;
        }

        return selectedLanguage;
    }
}