using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ConfirmPanel : PanelWithButton     //用于询问玩家是否确认自己的选择
{
    public static event Action OnYesButton;     //接收方为需要选择的所有UI界面（比如事件，拾取武器等）

    public Button YesButton;
    public Button NoButton;





    protected override void Awake()
    {
        //检查按钮组件是否存在
        if (YesButton == null || NoButton == null)
        {
            Debug.LogError("Some buttons are not assigned in the " + name);
            return;
        }

        //默认按钮为“确认”按钮
        firstSelectedButton = YesButton.gameObject;
    }

    private void Start()
    {
        //将按钮和函数绑定起来
        YesButton.onClick.AddListener(() => OnYesButtonClick());
        NoButton.onClick.AddListener(() => OnNoButtonClick());
    }




    private void OnYesButtonClick()
    {
        OnYesButton?.Invoke();      //回调事件，将逻辑绑定到这个事件，从而进行不同的逻辑
    }


    private void OnNoButtonClick()
    {
        //关闭界面
        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
    }    
}