using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



//用于老虎机的旋转
public class SlotMachinePanel : BasePanel
{
    public RectTransform SlotMachineContainer;          //储存图片的容器
    public float ScrollDuration = 10f;                  //一次旋转的时长
    public int TotalImages = 10;                        //图片的总数
    public int TargetIndex = 0;                         //最终停留的图片的索引



    private float m_ImageHeight;
    private bool m_IsScrolling = false;
    private Vector2 m_InitialPosition;







    #region Unity内部函数
    private void Start()
    {
        //计算每张图的高度
        m_ImageHeight = SlotMachineContainer.GetChild(0).GetComponent<RectTransform>().rect.height;

        //初始化起始位置
        m_InitialPosition = SlotMachineContainer.anchoredPosition;
    }

    private void Update()
    {
        //按空格开始旋转
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSlotMachine();
        }
    }
    #endregion


    #region 主要函数
    private void StartSlotMachine()
    {
        m_IsScrolling = true;

        ScrollToTarget();
    }

    private void ScrollToTarget()
    {
        if (!m_IsScrolling) return;

        //计算老虎机一次旋转的总长度
        float totalDistance = m_ImageHeight * TotalImages;

        //计算在目标图片停留时的位置
        float targetPositionY = m_InitialPosition.y - (m_ImageHeight * TargetIndex);

        //创建循环动画以进行老虎机旋转
        Sequence sequence = DOTween.Sequence();
        sequence.Append(SlotMachineContainer.DOAnchorPosY(m_InitialPosition.y - totalDistance, ScrollDuration).SetEase(Ease.InOutQuad))
                .Append(SlotMachineContainer.DOAnchorPosY(targetPositionY, 1f).SetEase(Ease.OutBounce))
                .OnComplete(() => m_IsScrolling = false);

        //每次老虎机旋转完成后将储存图片的容器的位置重置为初始位置
        sequence.OnStepComplete(() => SlotMachineContainer.anchoredPosition = m_InitialPosition);

        sequence.Play();
    }
    #endregion
}