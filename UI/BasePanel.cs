using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;



public class BasePanel : MonoBehaviour
{
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


    public float FadeDuration { get; protected set; }
    public float FadeInAlpha { get; protected set; }
    public float FadeOutAlpha { get; protected set; }


    protected PlayerInputHandler playerInputHandler;

    protected bool isTyping = false;        //表示是否正在显示文本

    protected bool isRemoved = false;       //表示UI是否被移除
    protected string panelName;







    protected virtual void Awake() 
    {
        //这个脚本跟打字和跳过对话相关
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();     //寻找有PlayerInputHandler组件的物体


        FadeDuration = 1f;
        FadeInAlpha = 1f;
        FadeOutAlpha = 0f;
    }








    public virtual void OpenPanel(string name)
    {
        this.panelName = name;

        //淡入界面
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);
    }

    public virtual void ClosePanel()
    {
        //Debug.Log("Panel is closed: " + panelName);

        isRemoved = true;

        //隐藏界面
        gameObject.SetActive(false);

        //销毁物体
        Destroy(gameObject);

        //释放物体和内存
        UIManager.Instance.ReleasePrefab(panelName);

        //从字典中移除，表示界面没打开
        UIManager.Instance.PanelDict.Remove(panelName);       
    }



    //用于界面的淡入/淡出（淡入时第四个参数应为真，淡出时为假）
    public virtual void Fade(CanvasGroup targetGroup, float targetAlpha, float duration, bool blocksRaycasts)
    {
        if(targetGroup != null)
        {
            targetGroup.DOFade(targetAlpha, duration).OnComplete(() =>
            {
                targetGroup.blocksRaycasts = blocksRaycasts;      //设置是否阻挡射线检测
            });
        }
    

        if (targetAlpha == FadeInAlpha && !UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            //添加缓存进字典，表示界面正在打开
            UIManager.Instance.PanelDict.Add(panelName, this);

            isRemoved = false;
        }

        else if (targetAlpha == FadeOutAlpha)
        {
            //从字典中移除缓存，表示界面没打开
            UIManager.Instance.PanelDict.Remove(panelName);

            isRemoved = true;
        }
    }


    /*
    protected void FadeIn(CanvasGroup thisCanvasGroup, float fadeDuration)   //用于界面的淡入
    {
        isRemoved = false;

        thisCanvasGroup.alpha = 0f;
        DOTween.To(() => thisCanvasGroup.alpha, x => thisCanvasGroup.alpha = x, 1f, fadeDuration);     //在1秒之内将透明度从0变为1，实现淡入效果
                
        //如果遮挡射线在取消状态，则重新激活
        if (thisCanvasGroup.blocksRaycasts == false)
        {
            thisCanvasGroup.blocksRaycasts = true;
        }

        //添加缓存进字典，表示界面正在打开
        if (!UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            UIManager.Instance.PanelDict.Add(panelName, this);
        }
    }

    public virtual void FadeOut(CanvasGroup thisCanvasGroup, float fadeDuration)   //用于界面的淡出（另一种关闭界面的方式，但不会销毁物体，只是通过更改透明度隐藏了起来）
    {
        isRemoved = true;      

        DOTween.To(() => thisCanvasGroup.alpha, x => thisCanvasGroup.alpha = x, 0, fadeDuration);   //在1秒之内将透明度从1变为0，实现淡出效果     


        //如果遮挡射线在激活状态，则取消激活
        if (thisCanvasGroup.blocksRaycasts == true)
        {
            thisCanvasGroup.blocksRaycasts = false;     //取消遮挡射线（因为物体没有被销毁。只是看不见了）
        }
           

        //从字典中移除缓存，表示界面没打开
        if (UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            UIManager.Instance.PanelDict.Remove(panelName);
        }
    }
    */




    protected virtual void DisplayText(TextMeshProUGUI textComponent)        //显示文本
    {
        if (textComponent != null)
        {
            isTyping = true;        //表示正在打字（防止正在打字时按空格会关闭UI）

            StartCoroutine(TypeText(textComponent, textComponent.text, 0.05f) );     //每隔0.05秒打一个字
        }
    }




    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float typeSpeed)
    {
        textComponent.text = "";     //先清空文本里的文字

        foreach (char letter in fullText)   //访问文本里的每一个元素（包括字。符号。空格等）
        {
            if (playerInputHandler.IsSpacePressed)       //玩家按下空格后直接显示全部文字
            {
                textComponent.text = fullText;
                yield break;
            }

            textComponent.text += letter;
            yield return new WaitForSeconds(typeSpeed);     //每当一个字打出来后，等待一段时间再继续运行（打下一个字）
        }

        isTyping = false;
    }



    protected IEnumerator ClosePanelAfterDelay(float delay)      //用于延迟一段时间后自动关闭界面
    {
        yield return new WaitForSeconds(delay);

        if (!isTyping)
        {
            ClosePanel();
        }
    }
}