using UnityEngine;
using UnityEngine.EventSystems;



public class PanelWithButton : BasePanel        //专门用于有按钮的界面UI
{
    //用于保证一直可以使用键盘操控按钮
    protected GameObject lastSelectedButton;


    protected virtual void Start()
    {
        if (lastSelectedButton != null)
        {
            //初始化按钮，随后将其设置到EventSystem
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }        
    }


    protected virtual void Update()
    {
        //如果因为鼠标点击的原因，导致无法用键盘选择按钮，则重新设置键盘默认按钮
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }

        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }
}