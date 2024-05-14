using TMPro;
using UnityEngine;
using Lean.Localization;



public class TaskPanel : BasePanel
{
    //任务文本对应的翻译文本的string
    public string TaskPhraseKey;

    

    TextMeshProUGUI m_TaskText;
    EnvironmentManager m_EnvironmentManager;       //此函数需要环境管理器中的回调函数，因此需要获取引用，而不能直接调用

    int m_FinishedTaskCount => EnvironmentManager.Instance.KilledEnemyCount;    //当前完成的任务数量
    int m_RequiredTaskCount => EnvironmentManager.Instance.RequiredEnemyCount;  //需要完成的任务数量





    protected override void Awake()
    {
        base.Awake();

        m_TaskText = GetComponentInChildren<TextMeshProUGUI>();     //从子物体那获取文本组件
        m_EnvironmentManager = FindAnyObjectByType<EnvironmentManager>();     //寻找环境管理器组件

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
        UpdateTaskText();       //游戏开始前正确显示任务数量
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





    //更新玩家的属性UI
    public void UpdateTaskText()
    {
        //获取翻译的文本组件
        string taskFormat = LeanLocalization.GetTranslationText(TaskPhraseKey);

        //赋值任务文本的数值
        m_TaskText.text = string.Format(taskFormat, m_FinishedTaskCount, m_RequiredTaskCount);            
    }
}