using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //ר�������а�ť�Ľ���UI
{
    //�򿪽������EventSystem��һ��ѡ��İ�ť
    protected GameObject firstSelectedButton;

    //��һ��ѡ��İ�ť
    protected GameObject lastSelectedButton;


    //���ڴ����������ڴ򿪵��а�ť�Ľ���
    List<PanelWithButton> m_OpenedPanelsWithButton = new List<PanelWithButton>();



    protected virtual void Update()
    {
        
        //�����ǰѡ��İ�ťΪ�գ�������Ϊ�����������رյȣ�
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            //��������Ĭ�ϰ�ť��ÿ�����治һ����
            //SetTopPriorityButton();
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }

        
        //������ѡ��ť��Ϊ����رն����ʱ�������¸�ֵ
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
        
    }


    protected virtual void OnEnable()
    {
        if (!m_OpenedPanelsWithButton.Contains(this) )
        {
            //Debug.Log("Panel with button added to the list: " + this.name);

            m_OpenedPanelsWithButton.Add(this);
        }

        //ÿ�ν������¼��غ������µİ�ť��EventSystem
        SetTopPriorityButton();       
    }

    protected void OnDisable()
    {
        if (m_OpenedPanelsWithButton.Contains(this))
        {
            //Debug.Log("Panel with button removed from the list: " + this.name);

            m_OpenedPanelsWithButton.Remove(this);
        }

        SetTopPriorityButton();
    }






    private void SetTopPriorityButton()
    {
        Debug.Log("The size of the OpenedPanelsWithButton List is : " + m_OpenedPanelsWithButton.Count);

        if (m_OpenedPanelsWithButton.Count != 0)
        {
            //�����ӽ��б�İ�ť�����һ�δ򿪵Ľ��棩����Ϊ������ȼ�
            lastSelectedButton = m_OpenedPanelsWithButton[m_OpenedPanelsWithButton.Count - 1].firstSelectedButton;
        }

        //����ȡ������������¼�ϵͳ��ĵ�ǰѡ��ť
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }
}