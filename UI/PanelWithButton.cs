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
    protected static List<PanelWithButton> openedPanelsWithButton = new List<PanelWithButton>();



    //用于表示是否有带按钮的界面打开
    bool m_IsPanelWithButtonOpened => openedPanelsWithButton.Count > 0;









    #region Unity内部函数
    /*
    protected override void Awake()
    {
        base.Awake();
        * 需要做的: 将所有按钮的选择,按下,切换的颜色和透明度根据类型进行统一
        * Highlighted Color和Selected Color的颜色和透明度: 颜色序列号: 6493DE, 透明度: 127
        * Pressed Color: 颜色序列号跟上面一样, 透明度: 45
        * Disabled Color: 颜色序列号: C8C8C8, 透明度: 127
        * 
        * 
        * 颜色序列号: 普通按钮: 6493DE, 退出按钮: DE7164
        * /    
    }
    */

    protected override void OnEnable()
    {
        base.OnEnable();

        //界面打开后加进列表
        if (!openedPanelsWithButton.Contains(this) )
        {
            //Debug.Log("Panel with button added to the list: " + this.name);

            openedPanelsWithButton.Add(this);
        }

        //每次界面重新加载后都设置新的按钮给EventSystem
        SetTopPriorityButton();
    }

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

    protected override void OnDisable()
    {
        base.OnDisable();

        //界面关闭后移出列表
        if (openedPanelsWithButton.Contains(this))
        {
            //Debug.Log("Panel with button removed from the list: " + this.name);

            openedPanelsWithButton.Remove(this);
        }

        SetTopPriorityButton();
    }
    #endregion


    #region 打开/关闭界面相关
    public override void Fade(CanvasGroup targetGroup, float targetAlpha, float duration, bool blocksRaycasts)
    {
        //有按钮的界面在淡入/淡出前，需要提前设置按钮是否可交互，否则会出现在淡出的过程中二次点击的情况
        targetGroup.interactable = blocksRaycasts;

        base.Fade(targetGroup, targetAlpha, duration, blocksRaycasts);
    }
    #endregion


    #region 主要函数
    protected virtual void SetTopPriorityButton()
    {
        //Debug.Log("The size of the OpenedPanelsWithButton List is : " + m_OpenedPanelsWithButton.Count);

        //PrintList();

        if (openedPanelsWithButton.Count > 0)
        {
            //将最后加进列表的按钮（最近一次打开的界面）设置为上一个选择的按钮
            lastSelectedButton = openedPanelsWithButton[openedPanelsWithButton.Count - 1].firstSelectedButton;
        }

        if (EventSystem.current != null)
        {
            //重置事件系统里的当前选择按钮
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }

        //根据是否有带按钮的界面打开来决定是否允许玩家移动和攻击
        SetBothMoveableAndAttackable(!m_IsPanelWithButtonOpened);
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
    #endregion
}