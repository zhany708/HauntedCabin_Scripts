using Lean.Localization;
using TMPro;
using UnityEngine;




//��Ȼ�ý���ֻ�������б�����Ϊ�˷�ֹ��Ҵ򿪽���������Զ��͹��������Լ̳���PanelWithButton
public class SettingPanel : PanelWithButton     
{
    public TMP_Dropdown Dropdown;       //�����б�
    public TMP_Text LabelText;          //�����б�����ʾ��ǰѡ����ı�

    public LeanLocalization Localization;       //���봦����������







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

        panelName = "test";     //��Ҫ���ģ��ǵ�ɾ��
    }

    private void Start()
    {
        PopulateDropdown();     //����ӵ�е�������������˵�

        Dropdown.onValueChanged.AddListener(ChangeLanguage);    //�������󶨵������˵�
    }

    protected override void Update() { }    //����������������Ҫ���ø�������߼���������д
    protected override void SetTopPriorityButton() { }





    private void PopulateDropdown()     //����ӵ�е�������������˵�
    {
        Dropdown.ClearOptions();    //���������ԭ����ѡ��

        foreach (var languageDict in LeanLocalization.CurrentLanguages)
        {
            string language = languageDict.Key;     //����value���ֵ��ǻ�ȡkey

            switch (language)       //���ı�ֱ����ʾ�Ǹ����ԣ��Ӷ�����ҿ��Ը���ʶ��
            {
                case "Chinese":
                    language = "����";
                    break;

                default:
                    language = "English";
                    break;
            }

            Dropdown.options.Add(new TMP_Dropdown.OptionData(language) );       //�����µ��ַ����������˵�
        }

        //���õ�ǰ���Զ�Ӧ��ֵ
        Dropdown.value = Dropdown.options.FindIndex(option => option.text == Localization.CurrentLanguage);
        LabelText.text = Localization.CurrentLanguage;      //��Label��ֵ��ǰ�����ԣ�������ʾ��ǰ�����˵�����ѡ���ֵ
    }


    private void ChangeLanguage(int index)
    {
        string selectedLanguage = Dropdown.options[index].text;     //���������˵���ֵ��ȡ��Ӧ���ı����ַ���

        switch (selectedLanguage)       ////�����ı�ֱ����ʾ�Ǹ����ԣ�������Ҫת��һ���ٸ�ֵ
        {
            case "����":
                selectedLanguage = "Chinese";
                break;

            default:
                selectedLanguage = "English";
                break;
        }


        Localization.SetCurrentLanguage(selectedLanguage);          //�л����ַ�����Ӧ������
    }
}