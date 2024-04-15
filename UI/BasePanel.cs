using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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



    protected PlayerInputHandler playerInputHandler;

    protected bool isTyping = false;        //表示是否正在显示文本

    protected bool isRemoved = false;       //表示UI是否被移除
    protected string panelName;








    protected virtual void Awake() 
    {
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();     //寻找有PlayerInputHandler组件的物体
    }




    public virtual void OpenPanel(string name)
    {
        this.panelName = name;
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
        UIManager.Instance.ReleaseUI(panelName);


        //从字典中移除，表示界面没打开
        if (UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            UIManager.Instance.PanelDict.Remove(panelName);
        }
    }


    protected void FadeIn(CanvasGroup thisCanvasGroup, float fadeDuration)   //用于界面的淡入
    {     
        thisCanvasGroup.alpha = 0f;
        DOTween.To(() => thisCanvasGroup.alpha, x => thisCanvasGroup.alpha = x, 1f, fadeDuration);     //在1秒之内将透明度从0变为1，实现淡入效果     
    }

    public virtual void FadeOut(CanvasGroup thisCanvasGroup, float fadeDuration)   //用于界面的淡出（另一种关闭界面的方式）
    {
        isRemoved = true;      

        DOTween.To(() => thisCanvasGroup.alpha, x => thisCanvasGroup.alpha = x, 0, fadeDuration);   //在1秒之内将透明度从1变为0，实现淡出效果     

        thisCanvasGroup.blocksRaycasts = false;     //取消遮挡射线（因为物体没有被销毁。只是看不见了）

        //移除缓存，表示界面没打开
        if (UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            UIManager.Instance.PanelDict.Remove(panelName);
        }
    }





    protected virtual void DisplayText(TextMeshProUGUI thisText)        //显示文本
    {
        if (thisText != null)
        {
            isTyping = true;        //表示正在打字（防止正在打字时按空格会关闭UI）

            TypeWriterEffect(thisText, 0.05f);     //每隔0.05秒打一个字
        }
    }

    protected void TypeWriterEffect(TextMeshProUGUI textComponent, float typeSpeed)      //打字机效果
    {
        StartCoroutine(TypeText(textComponent, textComponent.text, typeSpeed));
    }




    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float typeSpeed)      //用于打字机（每个字一个一个打出来）
    {
        textComponent.text = " ";       //先清空文本里的文字

        foreach (char letter in fullText.ToCharArray())     //访问文本里的每一个元素（包括字。符号。空格等）
        {
            if (playerInputHandler.IsSpacePressed)
            {
                textComponent.text = fullText;      //玩家按下空格后直接显示全部文字

                yield return new WaitForSeconds(0.5f);      //0.5秒后再设置布尔，防止显示完所有文字后UI立刻消失（详情看下面的ClosePanelAfterDelay函数）
                isTyping = false;

                yield break;    //退出协程
            }


            textComponent.text += letter;       

            yield return new WaitForSeconds(typeSpeed);     //每当一个字打出来后，等待一段时间再继续运行（打下一个字）
        }

        isTyping = false;       //文本全部打完后设置isTyping布尔为false
    }

    protected IEnumerator ClosePanelAfterDelay(float delay)      //用于延迟一段时间后自动关闭界面
    {
        float elapsedTime = 0;

        while (elapsedTime < delay)
        {
            if (playerInputHandler.IsSpacePressed && !isTyping)     //如果延迟过程中，且文本已显示完毕的情况下玩家按下空格，则立即关闭UI
            {
                ClosePanel();

                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ClosePanel();       //优先执行改写过的函数
    }
}