using TMPro;
using UnityEngine;
using Lean.Localization;



public class TaskPanel : BasePanel
{
    //任务文本对应的翻译文本的string
    public string TaskPhraseKey;

    

    TextMeshProUGUI m_TaskText;


    int m_FinishedTaskCount => HellsCall.Instance.GetFinishedRitualCount();    //当前完成的任务数量
    int m_RequiredTaskCount => HellsCall.Instance.GetNeededStoneNum();         //需要完成的任务数量





    protected override void Awake()
    {
        base.Awake();

        m_TaskText = GetComponentInChildren<TextMeshProUGUI>();     //从子物体那获取文本组件

        if (m_TaskText == null)
        {
            Debug.LogError("TaskText component is not assigned in the " + name);
            return;
        }
        
        if (TaskPhraseKey == "")
        {
            Debug.LogError("Lean Localization phrase key is not assigned or written in the " + name);
            return;
        }
    }

    private void Start()
    {
        UpdateTaskText();       //游戏开始前正确显示任务数量
    }


    private void OnEnable()
    {
        HellsCall.Instance.OnRitualFinished += UpdateTaskText;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        HellsCall.Instance.OnRitualFinished -= UpdateTaskText;
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