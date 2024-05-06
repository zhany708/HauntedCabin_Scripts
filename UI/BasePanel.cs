using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;



public class BasePanel : MonoBehaviour
{
    public event Action OnFadeInFinished;       //������ȫ����ʱ���õ��¼������շ�Ϊ����
    public event Action OnFadeOutFinished;       //������ȫ����ʱ���õ��¼������շ�Ϊ����



    private CanvasGroup m_CanvasGroup;
    public CanvasGroup CanvasGroup      //Lazy Loading��ֻ����Ҫʹ�����ʱ�ż����������������Awake������Ĭ�ϼ��أ�����ʡ�ڴ棩
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

    protected bool isTyping = false;        //��ʾ�Ƿ�������ʾ�ı�

    protected bool isRemoved = false;       //��ʾUI�Ƿ��Ƴ�
    protected string panelName;







    protected virtual void Awake() 
    {
        //����ű������ֺ������Ի����
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();     //Ѱ����PlayerInputHandler���������


        FadeDuration = 1f;
        FadeInAlpha = 1f;
        FadeOutAlpha = 0f;
    }


    protected virtual void OnDisable()
    {
        //���ֵ����Ƴ�����ʾ����û��
        UIManager.Instance.PanelDict.Remove(panelName);
    }





    public virtual void OpenPanel(string name)
    {
        this.panelName = name;

        //�������
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);
    }

    public virtual void ClosePanel()
    {
        //Debug.Log("Panel is closed: " + panelName);

        isRemoved = true;

        //���ؽ���
        gameObject.SetActive(false);

        //��������
        Destroy(gameObject);

        //�ͷ�������ڴ�
        UIManager.Instance.ReleasePrefab(panelName);     
    }



    //���ڽ���ĵ���/����������ʱ���ĸ�����ӦΪ�棬����ʱΪ�٣�
    public virtual void Fade(CanvasGroup targetGroup, float targetAlpha, float duration, bool blocksRaycasts)
    {
        if(targetGroup != null)
        {
            targetGroup.DOFade(targetAlpha, duration).OnComplete(() =>
            {
                targetGroup.blocksRaycasts = blocksRaycasts;      //�����Ƿ��赲���߼��

                //���泹�׵���ʱ
                if(targetAlpha == FadeOutAlpha)
                {
                    OnFadeOutFinished?.Invoke();
                }

                else
                {
                    OnFadeInFinished?.Invoke(); 
                }             
            });
        }
    
        else
        {
            Debug.LogError("CanvasGroup is not found in this panel: " + panelName);
        }




        if (targetAlpha == FadeInAlpha && !UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            //Debug.Log("Panel added to the PanelDict: " + panelName);

            //��ӻ�����ֵ䣬��ʾ�������ڴ�
            UIManager.Instance.PanelDict.Add(panelName, this);

            isRemoved = false;
        }

        else if (targetAlpha == FadeOutAlpha)
        {
            //Debug.Log("Panel removed from the PanelDict: " + panelName);

            //���ֵ����Ƴ����棬��ʾ����û��
            UIManager.Instance.PanelDict.Remove(panelName);

            isRemoved = true;
        }
    }
  




    protected virtual void DisplayText(TextMeshProUGUI textComponent)        //��ʾ�ı�
    {
        if (textComponent != null)
        {
            isTyping = true;        //��ʾ���ڴ��֣���ֹ���ڴ���ʱ���ո��ر�UI��

            StartCoroutine(TypeText(textComponent, textComponent.text, 0.05f) );     //ÿ��0.05���һ����
        }
    }




    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float typeSpeed)
    {
        textComponent.text = "";     //������ı��������

        foreach (char letter in fullText)   //�����ı����ÿһ��Ԫ�أ������֡����š��ո�ȣ�
        {
            if (playerInputHandler.IsSpacePressed)       //��Ұ��¿ո��ֱ����ʾȫ������
            {
                textComponent.text = fullText;
                break;
            }

            textComponent.text += letter;
            yield return new WaitForSeconds(typeSpeed);     //ÿ��һ���ִ�����󣬵ȴ�һ��ʱ���ټ������У�����һ���֣�
        }

        isTyping = false;
    }



    protected IEnumerator ClosePanelAfterDelay(float delay)      //�����ӳ�һ��ʱ����Զ��رս���
    {
        yield return new WaitForSeconds(delay);

        if (!isTyping)
        {
            ClosePanel();
        }
    }
}