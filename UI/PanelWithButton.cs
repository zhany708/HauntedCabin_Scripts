using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //专门用于有按钮的界面UI
{
    //用于表示是否有带按钮的界面打开
    public static bool IsPanelWithButtonOpened {  get; private set; }



    //打开界面后让EventSystem第一个选择的按钮
    protected GameObject firstSelectedButton;

    //上一个选择的按钮
    protected GameObject lastSelectedButton;


    //用于储存所有正在打开的有按钮的界面（加static从而让所有子类共用同一个列表）
    protected static List<PanelWithButton> m_OpenedPanelsWithButton = new List<PanelWithButton>();






    protected override void Awake()
    {
        base.Awake();

        IsPanelWithButtonOpened = false;
    }


    protected virtual void Update()
    {
        //先检查事件系统是否为空
        if (EventSystem.current != null)
        {
            //如果当前选择的按钮为空（可能因为鼠标点击，界面关闭等）
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                //重新设置上一个选择按钮
                EventSystem.current.SetSelectedGameObject(lastSelectedButton);
            }


            //持续更新上一个选择的按钮
            else
            {
                lastSelectedButton = EventSystem.current.currentSelectedGameObject;
            }
        } 
    }


    protected virtual void OnEnable()
    {
        //界面打开后加进列表
        if (!m_OpenedPanelsWithButton.Contains(this) )
        {
            //Debug.Log("Panel with button added to the list: " + this.name);

            m_OpenedPanelsWithButton.Add(this);
        }

        //每次界面重新加载后都设置新的按钮给EventSystem
        SetTopPriorityButton();

        //检查当前是否仍然有带按钮的界面打开
        CheckIsPanelWithButtonOpened();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //界面关闭后移出列表
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
            //将最后加进列表的按钮（最近一次打开的界面）设置为上一个选择的按钮
            lastSelectedButton = m_OpenedPanelsWithButton[m_OpenedPanelsWithButton.Count - 1].firstSelectedButton;
        }

        if (EventSystem.current != null)
        {
            //重置事件系统里的当前选择按钮
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
    }


    //通过列表的大小判断当前是否有带按钮的界面打开
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


    //用于调试
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