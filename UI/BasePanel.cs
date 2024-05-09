using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;



public class BasePanel : MonoBehaviour
{
    public event Action OnFadeInFinished;       //界面完全淡入时调用的事件，接收方为子类
    public event Action OnFadeOutFinished;       //界面完全淡出时调用的事件，接收方为子类



    private CanvasGroup m_CanvasGroup;
    public CanvasGroup CanvasGroup      //Lazy Loading（只在需要使用组件时才加载组件（而不是在Awake函数里默认加载），节省内存）
    {
        get
        {
            if (m_CanvasGroup == null)
            {
                m_CanvasGroup = GetComponent<CanvasGroup>();
            }
            return m_CanvasGroup;
        }
    }


    public float FadeDuration { get; protected set; } = 1f;
    public float FadeInAlpha { get; protected set; } = 1f;
    public float FadeOutAlpha { get; protected set; } = 0f;


    protected PlayerInputHandler playerInputHandler;

    protected bool isTyping = false;        //表示是否正在显示文本

    protected bool isRemoved = false;       //表示UI是否被移除
    protected string panelName;


    bool m_IsFading = false;    //表示是否正在淡入/出，用于防止界面正在淡入/出时就删除一些需要的组件




    protected virtual void Awake() 
    {
        //这个脚本跟打字和跳过对话相关
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();     //寻找有PlayerInputHandler组件的物体
    }


    protected virtual void OnDisable()
    {
        if (UIManager.Instance.PanelDict.ContainsKey(panelName) )
        {
            //从字典中移除，表示界面没打开
            UIManager.Instance.PanelDict.Remove(panelName);
        }     
    }





    public virtual void OpenPanel(string name)
    {
        panelName = name;

        //淡入界面
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);
    }

    public virtual void ClosePanel()
    {
        //Debug.Log("Panel is closed: " + panelName);

        isRemoved = true;     

        //安全销毁物体
        SafeDestroyPanel();

        //释放物体和内存
        UIManager.Instance.ReleasePrefab(panelName);     
    }



    //用于界面的淡入/淡出（淡入时第四个参数应为真，淡出时为假）
    public virtual void Fade(CanvasGroup targetGroup, float targetAlpha, float duration, bool blocksRaycasts)
    {
        if (targetGroup == null)
        {
            Debug.LogError("CanvasGroup is not found in panel: " + panelName);
            return;
        }

        m_IsFading = true;

        targetGroup.DOFade(targetAlpha, duration).OnComplete(() =>
        {
            targetGroup.blocksRaycasts = blocksRaycasts;    //设置是否阻挡射线检测
            m_IsFading = false;

            //在淡入的情况下
            if (targetAlpha != FadeOutAlpha)
            {
                OnFadeInFinished?.Invoke();

                isRemoved = false;

                //添加缓存进字典，表示界面正在打开
                UIManager.Instance.PanelDict[panelName] = this;
            }

            //淡出时
            else
            {
                OnFadeOutFinished?.Invoke();

                isRemoved = true;

                //从字典中移除缓存，表示界面没打开
                UIManager.Instance.PanelDict.Remove(panelName);
            }             
        });
    }
  




    protected virtual void DisplayText(TextMeshProUGUI textComponent)        //显示文本
    {
        if (textComponent != null)
        {          
            StartCoroutine(TypeText(textComponent, textComponent.text, 0.05f) );     //每隔0.05秒打一个字
        }
    }

    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float typeSpeed, Action onCompleted = null)
    {
        isTyping = true;        //表示正在打字（防止正在打字时按空格会关闭UI）


        int totalLength = fullText.Length;      //文本总长度，用于决定打字机何时结束
        int visibleCount = 0;                   //显示的文字数量


        textComponent.maxVisibleCharacters = 0;  //一开始什么都不显示

        while (visibleCount < totalLength)
        {
            if (playerInputHandler.IsSpacePressed)
            {
                textComponent.maxVisibleCharacters = totalLength;  //玩家按下空格后，瞬间显示所有文本
                break;  //退出循环
            }

            //检查是否在标签的开头
            if (fullText[visibleCount] == '<') 
            {
                //跳过整个标签，直到标签的结尾（也就是>符号）。跳过的方式为不更新可以显示的文字数量，但是依然增加visibleCount变量
                while (visibleCount < totalLength && fullText[visibleCount] != '>')
                {
                    visibleCount++;
                }
            }

            visibleCount++;  //增加可以显示的文字数量
            textComponent.maxVisibleCharacters = visibleCount;  //更新可以显示的文字数量

            yield return new WaitForSeconds(typeSpeed);  //等待一段时间后再打下一个字
        }

        isTyping = false;
        onCompleted?.Invoke();      //回调函数，用于某个文本全部显示完后执行一些逻辑
    }


    protected IEnumerator ClosePanelAfterDelay(float delay)      //用于延迟一段时间后自动关闭界面
    {
        yield return new WaitForSeconds(delay);

        if (!isTyping)
        {
            ClosePanel();
        }
        else
        {
            Debug.LogError("The panel is still typing, but you tried to close it.");
            yield break;
        }
    }




    private void SafeDestroyPanel()
    {
        if (m_IsFading)     //如果界面正在淡入/出时，则等待淡入/出结束后再删除物体
        {
            StartCoroutine(WaitForFadeEnd());
        }
        else    //不在淡入/出时立刻删除物体
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitForFadeEnd()
    {
        yield return new WaitWhile(() => m_IsFading);   //一直等待，直到淡入/出结束
        Destroy(gameObject);
    }
}