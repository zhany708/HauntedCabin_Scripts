using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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





    //储存所有当前界面进行过的协程，防止删除界面时某些协程仍在继续，占用内存（不加static，从而让每个界面都有一个单独的列表）
    protected List<Coroutine> generatedCoroutines = new List<Coroutine>();
    protected List<Tween> generatedDOTweens = new List<Tween>();        //跟上面同理



    protected float FadeDuration = 1f;          //默认淡入/出时间
    protected float FadeInAlpha = 1f;           //默认淡入值
    protected const float FadeOutAlpha = 0f;    //默认淡出值


    protected float typeSpeed = 0.05f;      //默认打字速度（每隔0.05秒打一个字）
 

    protected bool isTyping = false;        //表示是否正在显示文本
    protected bool isRemoved = false;       //表示UI是否被移除

    protected string panelName;             //界面名字






    protected virtual void Awake() { }


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

        ClearAllCoroutinesAndTweens();        //清除所有当前界面正在进行的协程

        //销毁物体
        Destroy(gameObject);

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


        
        Tween fadeTween = targetGroup.DOFade(targetAlpha, duration).OnComplete(() =>
        {
            targetGroup.blocksRaycasts = blocksRaycasts;    //设置是否阻挡射线检测

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

        generatedDOTweens.Add(fadeTween);      //将DOTween加进列表
    }
  


    //用于打字机效果
    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, Action onTypingCompleted = null)
    {
        isTyping = true;        //表示正在打字（防止正在打字时按空格会关闭UI）


        int totalLength = fullText.Length;      //文本总长度，用于决定打字机何时结束
        int visibleCount = 0;                   //显示的文字数量


        textComponent.maxVisibleCharacters = 0;  //一开始什么都不显示

        while (visibleCount < totalLength)
        {
            if (PlayerInputHandler.Instance.IsSpacePressed)
            {
                textComponent.maxVisibleCharacters = totalLength;  //玩家按下空格后，瞬间显示所有文本

                //等待0.15秒再退出，否则如果此函数结束后的下一个函数也需要按空格时，可能会导致按一次空格响应多个函数
                yield return new WaitForSeconds(0.15f);        
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
        onTypingCompleted?.Invoke();      //回调函数，用于某个文本全部显示完后执行一些逻辑
    }




    protected void ClearAllCoroutinesAndTweens()    //用于删除界面时检查是否有正在进行的协程和DOTween
    {
        foreach (var coroutine in generatedCoroutines)     //检阅列表中的所有协程
        {
            if (coroutine != null)      //检查协程是否存在（无论是否正在进行）
            {
                StopCoroutine(coroutine);
                //Debug.Log("One Coroutine is stopped.");
            }
        }

        foreach (var tween in generatedDOTweens)     //检阅列表中的所有协程
        {
            if (tween.active)      //检查DOTween是否正在进行
            {
                tween.Kill();
                //Debug.Log("One DOTween is stopped and killed.");
            }
        }

        generatedCoroutines.Clear();       //清除列表
        generatedDOTweens.Clear();       //清除列表
    }
}