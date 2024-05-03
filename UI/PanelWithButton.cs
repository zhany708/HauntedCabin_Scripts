using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //ר�������а�ť�Ľ���UI
{
    //���ڱ�ʾ�Ƿ��д���ť�Ľ����
    public static bool IsPanelWithButtonOpened {  get; private set; }



    //�򿪽������EventSystem��һ��ѡ��İ�ť
    protected GameObject firstSelectedButton;

    //��һ��ѡ��İ�ť
    protected GameObject lastSelectedButton;


    //���ڴ����������ڴ򿪵��а�ť�Ľ��棨��static�Ӷ����������๲��ͬһ���б�
    protected static List<PanelWithButton> m_OpenedPanelsWithButton = new List<PanelWithButton>();






    protected override void Awake()
    {
        base.Awake();

        IsPanelWithButtonOpened = false;
    }


    protected virtual void Update()
    {
        //�ȼ���¼�ϵͳ�Ƿ�Ϊ��
        if (EventSystem.current != null)
        {
            //�����ǰѡ��İ�ťΪ�գ�������Ϊ�����������رյȣ�
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                //����������һ��ѡ��ť
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            }


            //����������һ��ѡ��İ�ť
            else
            {
                lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            }
        } 
    }


    protected virtual void OnEnable()
    {
        //����򿪺�ӽ��б�
        if (!m_OpenedPanelsWithButton.Contains(this) )
        {
            //Debug.Log("Panel with button added to the list: " + this.name);

            m_OpenedPanelsWithButton.Add(this);
        }

        //ÿ�ν������¼��غ������µİ�ť��EventSystem
        SetTopPriorityButton();

        //��鵱ǰ�Ƿ���Ȼ�д���ť�Ľ����
        CheckIsPanelWithButtonOpened();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //����رպ��Ƴ��б�
        if (m_OpenedPanelsWithButton.Contains(this))
        {
            //Debug.Log("Panel with button removed from the list: " + this.name);

            m_OpenedPanelsWithButton.Remove(this);
        }

        SetTopPriorityButton();
        CheckIsPanelWithButtonOpened();
    }






    private void SetTopPriorityButton()
    {
        //Debug.Log("The size of the OpenedPanelsWithButton List is : " + m_OpenedPanelsWithButton.Count);

        //PrintList();

        if (m_OpenedPanelsWithButton.Count != 0)
        {
            //�����ӽ��б�İ�ť�����һ�δ򿪵Ľ��棩����Ϊ��һ��ѡ��İ�ť
            lastSelectedButton = m_OpenedPanelsWithButton[m_OpenedPanelsWithButton.Count - 1].firstSelectedButton;
        }

        if (EventSystem.current != null)
        {
            //�����¼�ϵͳ��ĵ�ǰѡ��ť
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
    }


    //ͨ���б�Ĵ�С�жϵ�ǰ�Ƿ��д���ť�Ľ����
    private void CheckIsPanelWithButtonOpened()
    { 
        if (m_OpenedPanelsWithButton.Count != 0)
        {
            IsPanelWithButtonOpened = true;
        }

        else
        {
            IsPanelWithButtonOpened = false;
        }
    }


    //���ڵ���
    private void PrintList()
    {
        if (m_OpenedPanelsWithButton.Count != 0)
        {
            for (int i = 0; i < m_OpenedPanelsWithButton.Count; i++)
            {
                Debug.Log("The OpenedPanelsWithButton List contains this panel : " + m_OpenedPanelsWithButton[i].name);
            }
        }
    }
}