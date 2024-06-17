using UnityEngine.UI;
using UnityEngine;
using System;



public class InteractPanel : BasePanel     //互动按键，给予玩家自己决定是否打开某些界面（比如拾取武器），而不是触发了触发器后自动打开界面
{
    public static event Action OnInteractKeyPressed;     //接收方为需要选择的所有UI界面（比如事件中的选项，拾取武器等）





    private void Update() 
    {
        if (PlayerInputHandler.Instance.IsInteractKeyPressed)       //持续检查玩家是否按下互动按键
        {
            OnInteractKeyPressed?.Invoke();     //调用事件
        }
    }




    //需要做的：玩家靠近一些物体后打开此界面，离开物体后淡出此界面。且此界面的坐标应更改为物体坐标（随触发的物体改变）
    public override void ClosePanel()
    {
        //淡出界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }
}