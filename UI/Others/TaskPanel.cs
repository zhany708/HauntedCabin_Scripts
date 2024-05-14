using TMPro;
using UnityEngine;
using Lean.Localization;



public class TaskPanel : BasePanel
{
    //�����ı���Ӧ�ķ����ı���string
    public string TaskPhraseKey;

    

    TextMeshProUGUI m_TaskText;
    EnvironmentManager m_EnvironmentManager;       //�˺�����Ҫ�����������еĻص������������Ҫ��ȡ���ã�������ֱ�ӵ���

    int m_FinishedTaskCount => EnvironmentManager.Instance.KilledEnemyCount;    //��ǰ��ɵ���������
    int m_RequiredTaskCount => EnvironmentManager.Instance.RequiredEnemyCount;  //��Ҫ��ɵ���������





    protected override void Awake()
    {
        base.Awake();

        m_TaskText = GetComponentInChildren<TextMeshProUGUI>();     //���������ǻ�ȡ�ı����
        m_EnvironmentManager = FindAnyObjectByType<EnvironmentManager>();     //Ѱ�һ������������

        if (m_TaskText == null || m_EnvironmentManager == null)
        {
            Debug.LogError("Some components are not assigned in the TaskPanel.");
            return;
        }

        if (TaskPhraseKey == "")
        {
            Debug.LogError("Lean Localization phrase key is not assigned or written in the TaskPanel.");
            return;
        }
    }

    private void Start()
    {
        UpdateTaskText();       //��Ϸ��ʼǰ��ȷ��ʾ��������
    }


    private void OnEnable()
    {
        m_EnvironmentManager.OnEnemyKilled += UpdateTaskText;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        m_EnvironmentManager.OnEnemyKilled -= UpdateTaskText;
    }





    //������ҵ�����UI
    public void UpdateTaskText()
    {
        //��ȡ������ı����
        string taskFormat = LeanLocalization.GetTranslationText(TaskPhraseKey);

        //��ֵ�����ı�����ֵ
        m_TaskText.text = string.Format(taskFormat, m_FinishedTaskCount, m_RequiredTaskCount);            
    }
}