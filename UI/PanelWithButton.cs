using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //专门用于有按钮的界面UI
{
    



    //打开界面后让EventSystem第一个选择的按钮
    protected GameObject firstSelectedButton = null;

    //上一个选择的按钮
    protected GameObject lastSelectedButton;


    //用于储存所有正在打开的有按钮的界面（加static从而让所有子类共用同一个列表）
    protected static List<PanelWithButton> m_OpenedPanelsWithButton = new List<PanelWithButton>();



    //用于表示是否有带按钮的界面打开
    bool m_IsPanelWithButtonOpened => m_OpenedPanelsWithButton.Count > 0;





    protected virtual void Update()
    {      
        //先检查事件系统是否为空
        if (EventSystem.current != null)
        {
            //如果当前选择的按钮为空（可能因为鼠标点击，界面关闭等）
            if (EventSystem.current.currentSelectedGameObject == null && lastSelectedButton != null)
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
    }





    public override void Fade(CanvasGroup targetGroup, float targetAlpha, float duration, bool blocksRaycasts)
    {
        //有按钮的界面在淡入/淡出前，需要提前设置按钮是否可交互，否则会出现在淡出的过程中二次点击的情况
        targetGroup.interactable = blocksRaycasts;

        base.Fade(targetGroup, targetAlpha, duration, blocksRaycasts);
    }



    protected virtual void SetTopPriorityButton()
    {
        //Debug.Log("The size of the OpenedPanelsWithButton List is : " + m_OpenedPanelsWithButton.Count);

        //PrintList();

        if (m_OpenedPanelsWithButton.Count > 0)
        {
            //将最后加进列表的按钮（最近一次打开的界面）设置为上一个选择的按钮
            lastSelectedButton = m_OpenedPanelsWithButton[m_OpenedPanelsWithButton.Count - 1].firstSelectedButton;
        }

        if (EventSystem.current != null)
        {
            //重置事件系统里的当前选择按钮
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }

        //根据是否有带按钮的界面打开来决定是否允许玩家移动和攻击
        SetBothMoveableAndAttackable(!m_IsPanelWithButtonOpened);
    }




    protected void ResetGameSystems()
    {
        //重置游戏的各种系统
        EventManager.Instance.ResetGame();
        UIManager.Instance.ResetGame();
        EnemyPool.Instance.ResetGame();
    }

    /*
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
    */
}