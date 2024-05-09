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


    public float FadeDuration { get; protected set; } = 1f;
    public float FadeInAlpha { get; protected set; } = 1f;
    public float FadeOutAlpha { get; protected set; } = 0f;


    protected PlayerInputHandler playerInputHandler;

    protected bool isTyping = false;        //��ʾ�Ƿ�������ʾ�ı�

    protected bool isRemoved = false;       //��ʾUI�Ƿ��Ƴ�
    protected string panelName;


    bool m_IsFading = false;    //��ʾ�Ƿ����ڵ���/�������ڷ�ֹ�������ڵ���/��ʱ��ɾ��һЩ��Ҫ�����




    protected virtual void Awake() 
    {
        //����ű������ֺ������Ի����
        playerInputHandler = FindObjectOfType<PlayerInputHandler>();     //Ѱ����PlayerInputHandler���������
    }


    protected virtual void OnDisable()
    {
        if (UIManager.Instance.PanelDict.ContainsKey(panelName) )
        {
            //���ֵ����Ƴ�����ʾ����û��
            UIManager.Instance.PanelDict.Remove(panelName);
        }     
    }





    public virtual void OpenPanel(string name)
    {
        panelName = name;

        //�������
        Fade(CanvasGroup, FadeInAlpha, FadeDuration, true);
    }

    public virtual void ClosePanel()
    {
        //Debug.Log("Panel is closed: " + panelName);

        isRemoved = true;     

        //��ȫ��������
        SafeDestroyPanel();

        //�ͷ�������ڴ�
        UIManager.Instance.ReleasePrefab(panelName);     
    }



    //���ڽ���ĵ���/����������ʱ���ĸ�����ӦΪ�棬����ʱΪ�٣�
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
            targetGroup.blocksRaycasts = blocksRaycasts;    //�����Ƿ��赲���߼��
            m_IsFading = false;

            //�ڵ���������
            if (targetAlpha != FadeOutAlpha)
            {
                OnFadeInFinished?.Invoke();

                isRemoved = false;

                //��ӻ�����ֵ䣬��ʾ�������ڴ�
                UIManager.Instance.PanelDict[panelName] = this;
            }

            //����ʱ
            else
            {
                OnFadeOutFinished?.Invoke();

                isRemoved = true;

                //���ֵ����Ƴ����棬��ʾ����û��
                UIManager.Instance.PanelDict.Remove(panelName);
            }             
        });
    }
  




    protected virtual void DisplayText(TextMeshProUGUI textComponent)        //��ʾ�ı�
    {
        if (textComponent != null)
        {          
            StartCoroutine(TypeText(textComponent, textComponent.text, 0.05f) );     //ÿ��0.05���һ����
        }
    }

    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, float typeSpeed, Action onCompleted = null)
    {
        isTyping = true;        //��ʾ���ڴ��֣���ֹ���ڴ���ʱ���ո��ر�UI��


        int totalLength = fullText.Length;      //�ı��ܳ��ȣ����ھ������ֻ���ʱ����
        int visibleCount = 0;                   //��ʾ����������


        textComponent.maxVisibleCharacters = 0;  //һ��ʼʲô������ʾ

        while (visibleCount < totalLength)
        {
            if (playerInputHandler.IsSpacePressed)
            {
                textComponent.maxVisibleCharacters = totalLength;  //��Ұ��¿ո��˲����ʾ�����ı�
                break;  //�˳�ѭ��
            }

            //����Ƿ��ڱ�ǩ�Ŀ�ͷ
            if (fullText[visibleCount] == '<') 
            {
                //����������ǩ��ֱ����ǩ�Ľ�β��Ҳ����>���ţ��������ķ�ʽΪ�����¿�����ʾ������������������Ȼ����visibleCount����
                while (visibleCount < totalLength && fullText[visibleCount] != '>')
                {
                    visibleCount++;
                }
            }

            visibleCount++;  //���ӿ�����ʾ����������
            textComponent.maxVisibleCharacters = visibleCount;  //���¿�����ʾ����������

            yield return new WaitForSeconds(typeSpeed);  //�ȴ�һ��ʱ����ٴ���һ����
        }

        isTyping = false;
        onCompleted?.Invoke();      //�ص�����������ĳ���ı�ȫ����ʾ���ִ��һЩ�߼�
    }


    protected IEnumerator ClosePanelAfterDelay(float delay)      //�����ӳ�һ��ʱ����Զ��رս���
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
        if (m_IsFading)     //����������ڵ���/��ʱ����ȴ�����/����������ɾ������
        {
            StartCoroutine(WaitForFadeEnd());
        }
        else    //���ڵ���/��ʱ����ɾ������
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator WaitForFadeEnd()
    {
        yield return new WaitWhile(() => m_IsFading);   //һֱ�ȴ���ֱ������/������
        Destroy(gameObject);
    }
}