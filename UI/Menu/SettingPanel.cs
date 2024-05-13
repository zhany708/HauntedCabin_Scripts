using Lean.Localization;
using TMPro;
using UnityEngine;




//虽然该界面只有下拉列表，但是为了防止玩家打开界面后自由自动和攻击，所以继承自PanelWithButton
public class SettingPanel : PanelWithButton     
{
    public TMP_Dropdown Dropdown;       //下拉列表
    public TMP_Text LabelText;          //下拉列表中显示当前选项的文本

    public LeanLocalization Localization;       //翻译处理器的引用







    protected override void Awake()
    {
        if (Dropdown == null || LabelText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the SettingPanel.");
            return;
        }

        if (Localization == null)
        {
            Debug.LogError("LeanLocalization component is not assigned in the SettingPanel.");
            return;
        }

        panelName = "test";     //需要做的：记得删掉
    }

    private void Start()
    {
        PopulateDropdown();     //根据拥有的语言填充下拉菜单

        Dropdown.onValueChanged.AddListener(ChangeLanguage);    //将函数绑定到下拉菜单
    }

    protected override void Update() { }    //这两个函数都不需要调用父类里的逻辑，所以重写
    protected override void SetTopPriorityButton() { }





    private void PopulateDropdown()     //根据拥有的语言填充下拉菜单
    {
        Dropdown.ClearOptions();    //先清空所有原本的选项

        foreach (var languageDict in LeanLocalization.CurrentLanguages)
        {
            string language = languageDict.Key;     //根据value从字典那获取key

            switch (language)       //让文本直接显示那个语言，从而让玩家可以更快识别
            {
                case "Chinese":
                    language = "中文";
                    break;

                default:
                    language = "English";
                    break;
            }

            Dropdown.options.Add(new TMP_Dropdown.OptionData(language) );       //创建新的字符串给下拉菜单
        }

        //设置当前语言对应的值
        Dropdown.value = Dropdown.options.FindIndex(option => option.text == Localization.CurrentLanguage);
        LabelText.text = Localization.CurrentLanguage;      //给Label赋值当前的语言，用于显示当前下拉菜单正在选择的值
    }


    private void ChangeLanguage(int index)
    {
        string selectedLanguage = Dropdown.options[index].text;     //根据下拉菜单的值获取对应的文本的字符串

        switch (selectedLanguage)       ////由于文本直接显示那个语言，所以需要转换一下再赋值
        {
            case "中文":
                selectedLanguage = "Chinese";
                break;

            default:
                selectedLanguage = "English";
                break;
        }


        Localization.SetCurrentLanguage(selectedLanguage);          //切换到字符串对应的语言
    }
}