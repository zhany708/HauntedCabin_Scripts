using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




//��Ȼ�ý���ֻ�������б�����Ϊ�˷�ֹ��Ҵ򿪽���������Զ��͹��������Լ̳���PanelWithButton
public class SettingPanel : PanelWithButton     
{
    public TMP_Dropdown Dropdown;       //�����б�
    public TMP_Text LabelText;          //�����б�����ʾ��ǰѡ����ı�
    public Button CloseButton;          //���ڹرս���İ�ť

    LeanLocalization m_Localization;       //���봦����������







    protected override void Awake()
    {
        if (Dropdown == null || LabelText == null || CloseButton == null)
        {
            Debug.LogError("Some TMP components are not assigned in the SettingPanel.");
            return;
        }


        m_Localization = FindObjectOfType<LeanLocalization>();  //Ѱ�ҷ������

        if (m_Localization == null)
        {
            Debug.LogError("LeanLocalization component is not assigned in the SettingPanel.");
            return;
        }
    }

    private void Start()
    {
        PopulateDropdown();     //����ӵ�е�������������˵�

        Dropdown.onValueChanged.AddListener(ChangeLanguage);    //�������󶨵������˵�
        CloseButton.onClick.AddListener(() => Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false) );   //�������󶨵���ť
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







    private void PopulateDropdown()     //����ӵ�е�������������˵�
    {
        Dropdown.ClearOptions();    //���������ԭ����ѡ��

        foreach (var languageDict in LeanLocalization.CurrentLanguages)
        {
            string language = languageDict.Key;     //����value���ֵ��ǻ�ȡkey         

            //ת�����Ժ󣬴����µ��ַ����������˵�
            Dropdown.options.Add(new TMP_Dropdown.OptionData(LanguageTransform(language)) );      
        }

        //ת�����Ժ����õ�ǰ���Զ�Ӧ��ֵ
        Dropdown.value = Dropdown.options.FindIndex(option => option.text == LanguageTransform(m_Localization.CurrentLanguage) );
        //ת�����Ժ󣬸�Label��ֵ��ǰ�����ԣ�������ʾ��ǰ�����˵�����ѡ���ֵ
        LabelText.text = LanguageTransform(m_Localization.CurrentLanguage);      
    }


    private void ChangeLanguage(int index)
    {
        string selectedLanguage = Dropdown.options[index].text;     //���������˵���ֵ��ȡ��Ӧ���ı����ַ���

        //ת�����Ժ��л����ַ�����Ӧ������
        m_Localization.SetCurrentLanguage(LanguageTransform(selectedLanguage) );
    }


    //���ı�ֱ����ʾ�Ǹ����ԣ��Ӷ�����ҿ��Ը���ʶ�𣻻���ת������
    private string LanguageTransform(string selectedLanguage)
    {
        switch (selectedLanguage)
        {
            case "Chinese":
                selectedLanguage = "����";
                break;

            case "����":
                selectedLanguage = "Chinese";
                break;

            default:
                selectedLanguage = "English";       //Ĭ�Ϸ���English
                break;
        }

        return selectedLanguage;
    }
}