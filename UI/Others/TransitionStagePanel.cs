using TMPro;
using DG.Tweening;
using UnityEngine;


public class TransitionStagePanel : BasePanel
{
    TextMeshProUGUI m_TransitionStageText;      //文本组件

    float m_DisplayDuration = 10f;





    protected override void Awake()
    {
        base.Awake();

        m_TransitionStageText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        OpenPanel("TransitionStagePanel");
    }




    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        FadeIn(CanvasGroup, 1f);     //淡入

        DisplayText(m_TransitionStageText);     //显示文本
        StartCoroutine(ClosePanelAfterDelay(m_DisplayDuration));     //显示一定时间后自动关闭界面
    }

    public override void ClosePanel()
    {
        FadeOut(CanvasGroup, 1f);     //淡出
    }



    public override void FadeOut(CanvasGroup thisCanvasGroup, float fadeDuration)
    {
        if (UIManager.Instance.PanelDict.ContainsKey(panelName))
        {
            UIManager.Instance.PanelDict.Remove(panelName);
        }


        DOTween.To(() => thisCanvasGroup.alpha, x => thisCanvasGroup.alpha = x, 0, fadeDuration).OnComplete(      //执行完后销毁物体
            () =>
            {
                Destroy(gameObject);

                UIManager.Instance.ReleaseUI(panelName);
            }
        );
    }
}