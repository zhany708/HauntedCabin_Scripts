using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //专门用于有按钮的界面UI
{
    //打开界面后让EventSystem第一个选择的按钮
    protected GameObject firstSelectedButton;

    //上一个选择的按钮
    protected GameObject lastSelectedButton;


    //用于储存所有正在打开的有按钮的界面
    List<PanelWithButton> m_OpenedPanelsWithButton = new List<PanelWithButton>();



    protected virtual void Update()
    {
        
        //如果当前选择的按钮为空（可能因为鼠标点击，界面关闭等）
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            //重新设置默认按钮（每个界面不一样）
            //SetTopPriorityButton();
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }

        
        //当优先选择按钮因为界面关闭而清空时，则重新赋值
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

        //每次界面重新加载后都设置新的按钮给EventSystem
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
            //将最后加进列表的按钮（最近一次打开的界面）设置为最高优先级
            lastSelectedButton = m_OpenedPanelsWithButton[m_OpenedPanelsWithButton.Count - 1].firstSelectedButton;
        }

        //界面取消激活后重置事件系统里的当前选择按钮
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }
}