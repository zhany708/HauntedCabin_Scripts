using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;




public class BasePanel : MonoBehaviour
{
    public event Action OnFadeInFinished;       //������ȫ����ʱ���õ��¼������շ�Ϊ����
    public event Action OnFadeOutFinished;       //������ȫ����ʱ���õ��¼������շ�Ϊ����


    public static bool IsPlayerMoveable { get; protected set; } = true;     //�Ƿ���������ƶ�
    public static bool IsPlayerAttackable { get; protected set; } = true;   //�Ƿ�������ҹ���


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





    //�������е�ǰ������й���Э�̣���ֹɾ������ʱĳЩЭ�����ڼ�����ռ���ڴ棨����static���Ӷ���ÿ�����涼��һ���������б�
    protected List<Coroutine> generatedCoroutines = new List<Coroutine>();
    protected List<Tween> generatedDOTweens = new List<Tween>();        //������ͬ��



    protected float FadeDuration = 1f;          //Ĭ�ϵ���/��ʱ��
    protected float FadeInAlpha = 1f;           //Ĭ�ϵ���ֵ
    protected const float FadeOutAlpha = 0f;    //Ĭ�ϵ���ֵ


    protected float typeSpeed = 0.05f;      //Ĭ�ϴ����ٶȣ�ÿ��0.05���һ���֣�
 

    protected bool isTyping = false;        //��ʾ�Ƿ�������ʾ�ı�
    protected bool isRemoved = false;       //��ʾUI�Ƿ��Ƴ�

    protected string panelName;             //��������






    protected virtual void Awake() { }


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

        ClearAllCoroutinesAndTweens();        //������е�ǰ�������ڽ��е�Э��

        //��������
        Destroy(gameObject);

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


        
        Tween fadeTween = targetGroup.DOFade(targetAlpha, duration).OnComplete(() =>
        {
            targetGroup.blocksRaycasts = blocksRaycasts;    //�����Ƿ��赲���߼��

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

        generatedDOTweens.Add(fadeTween);      //��DOTween�ӽ��б�
    }
  


    //���ڴ��ֻ�Ч��
    protected IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText, Action onTypingCompleted = null)
    {
        isTyping = true;        //��ʾ���ڴ��֣���ֹ���ڴ���ʱ���ո��ر�UI��


        int totalLength = fullText.Length;      //�ı��ܳ��ȣ����ھ������ֻ���ʱ����
        int visibleCount = 0;                   //��ʾ����������


        textComponent.maxVisibleCharacters = 0;  //һ��ʼʲô������ʾ

        while (visibleCount < totalLength)
        {
            if (PlayerInputHandler.Instance.IsSpacePressed)
            {
                textComponent.maxVisibleCharacters = totalLength;  //��Ұ��¿ո��˲����ʾ�����ı�

                //�ȴ�0.15�����˳�����������˺������������һ������Ҳ��Ҫ���ո�ʱ�����ܻᵼ�°�һ�οո���Ӧ�������
                yield return new WaitForSeconds(0.15f);        
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
        onTypingCompleted?.Invoke();      //�ص�����������ĳ���ı�ȫ����ʾ���ִ��һЩ�߼�
    }




    protected void ClearAllCoroutinesAndTweens()    //����ɾ������ʱ����Ƿ������ڽ��е�Э�̺�DOTween
    {
        foreach (var coroutine in generatedCoroutines)     //�����б��е�����Э��
        {
            if (coroutine != null)      //���Э���Ƿ���ڣ������Ƿ����ڽ��У�
            {
                StopCoroutine(coroutine);
                //Debug.Log("One Coroutine is stopped.");
            }
        }

        foreach (var tween in generatedDOTweens)     //�����б��е�����Э��
        {
            if (tween.active)      //���DOTween�Ƿ����ڽ���
            {
                tween.Kill();
                //Debug.Log("One DOTween is stopped and killed.");
            }
        }

        generatedCoroutines.Clear();       //����б�
        generatedDOTweens.Clear();       //����б�
    }



    protected void SetBothMoveableAndAttackable(bool isTrue)    //�����Ƿ���������ƶ��͹���
    {
        IsPlayerMoveable = isTrue;
        IsPlayerAttackable = isTrue;
    }
}