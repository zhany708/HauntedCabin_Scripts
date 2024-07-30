using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Lean.Localization;
using TMPro;



//用于老虎机的旋转
public class SlotMachinePanel : BasePanel
{
    public RectTransform SlotMachineContainer;      //储存图片的容器
    public RectTransform SlotMachineViewport;       //允许玩家看到的一小部分

    public float ScrollDuration = -1f;              //一次旋转的时长
    public int TotalImages = -1;                    //图片的总数
    public int TargetIndex = -1;                    //最终停留的图片的索引
    public float ScrollSpeed = -1f;                 //老虎机滚动的速度



    Vector2[] m_ImageArray;                         //储存所有图片的坐标，用于Debug

    Vector2 m_InitialContainerPos;                  //容器的初始位置
    float m_ImageHeight;                            //每个图片的高
    bool m_IsScrolling = false;
    Sequence m_ScrollSequence;






    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();


        if (SlotMachineContainer == null || SlotMachineViewport == null || ScrollDuration <= 0 || TotalImages <= 0 || TargetIndex <= 0 || ScrollSpeed <= 0)
        {
            Debug.LogError("Some components are not assigned in the " + gameObject.name);
            return;
        }

        //初始化数组
        m_ImageArray = new Vector2[TotalImages];

        m_InitialContainerPos = SlotMachineContainer.anchoredPosition;      //初始化容器的坐标
    }

    private void Start()
    {
        //计算每个图片的高度
        m_ImageHeight = SlotMachineContainer.GetChild(0).GetComponent<RectTransform>().rect.height;
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
        if (m_IsScrolling) return;


        //记录所有图片的初始坐标
        for (int i = 0; i < SlotMachineContainer.childCount; i++)
        {
            RectTransform imageRect = SlotMachineContainer.GetChild(i).GetComponent<RectTransform>();

            m_ImageArray[i] = imageRect.anchoredPosition;
        }

        //Debug.Log($"Before: The position of target image is {m_ImageArray[TargetIndex]} and the position of Container is {SlotMachineContainer.anchoredPosition}");

        m_IsScrolling = true;

        ScrollContinuously();
    }


    //用于老虎机的滚动
    private void ScrollContinuously()
    {
        //计算老虎机一次旋转的总长度（如果图片间距大于0的话，这里要进行改动，否则会导致旋转结束后图片位置与初始位置不一致）
        float totalDistance = m_ImageHeight * TotalImages * ScrollSpeed;


        //创建循环动画以进行老虎机旋转
        m_ScrollSequence = DOTween.Sequence();

        m_ScrollSequence.Append(SlotMachineContainer.DOAnchorPosY(SlotMachineContainer.anchoredPosition.y - totalDistance, ScrollDuration).SetEase(Ease.Linear))
                .OnUpdate(() =>
                {
                    CheckAndRepositionImages();     //持续检查并更新图片的坐标


                    //不能开始旋转后就立刻更改，否则玩家会看到
                    if (SlotMachineContainer.anchoredPosition.y <= -100)
                    {
                        ChangeTargetImageColor(Color.blue);     //更改目标图片的颜色
                    }
                })
                 
                .OnComplete(() =>
                {
                    m_IsScrolling = false;

                    ResetImagePositions();      //重置坐标
                    //Debug.Log($"After: The position of target image is {m_ImageArray[TargetIndex]} and the position of Container is {SlotMachineContainer.anchoredPosition}");                 
                });
        
             
        m_ScrollSequence.Play();
    }

    /*
    //用于老虎机的滚动
    private void ScrollContinuously()
    {
        //计算老虎机一次旋转的总长度
        float totalDistance = m_ImageHeight * TotalImages * ScrollSpeed;

        //设置初始快速滚动阶段的时长和慢速滚动阶段的时长
        float fastScrollDuration = ScrollDuration * 0.8f;   //前80%的时长
        float slowScrollDuration = ScrollDuration * 0.2f;   //后20%的时长

        //计算快速滚动阶段的距离和慢速滚动阶段的距离
        float fastScrollDistance = totalDistance * 0.8f;
        float slowScrollDistance = totalDistance * 0.2f;

        //创建循环动画以进行老虎机旋转
        m_ScrollSequence = DOTween.Sequence();

        //初始快速滚动阶段（Ease.Linear的速度是均匀的，而Ease.OutCubic的速度是缓慢下降的）
        m_ScrollSequence.Append(SlotMachineContainer.DOAnchorPosY(SlotMachineContainer.anchoredPosition.y - fastScrollDistance, fastScrollDuration).SetEase(Ease.Linear))
            .Append(SlotMachineContainer.DOAnchorPosY(SlotMachineContainer.anchoredPosition.y - slowScrollDistance, slowScrollDuration).SetEase(Ease.OutCubic))
            .OnUpdate(() =>
            {
                CheckAndRepositionImages();     //持续检查并更新图片的坐标

                //不能开始旋转后就立刻更改，否则玩家会看到
                if (SlotMachineContainer.anchoredPosition.y <= -100)
                {
                    ChangeTargetImageColor(Color.blue);     //更改目标图片的颜色
                }
            })
            .OnComplete(() =>
            {
                m_IsScrolling = false;
                ResetImagePositions();           //重置所有图片的坐标
            });

        m_ScrollSequence.Play();
    }
    */



    //用于无限循环滚动图片
    private void CheckAndRepositionImages()
    {
        //由于老虎机的旋转是移动SlotMachineContainer，所以检查坐标时必须加上容器的坐标
        RectTransform slotMachineRect = SlotMachineContainer.GetComponent<RectTransform>();

        for (int i = 0; i < SlotMachineContainer.childCount; i++)
        {
            RectTransform imageRect = SlotMachineContainer.GetChild(i).GetComponent<RectTransform>();
            if (imageRect.anchoredPosition.y + slotMachineRect.anchoredPosition.y < -m_ImageHeight * (TotalImages))
            {
                //将图片移至最上方
                float newY = imageRect.anchoredPosition.y + m_ImageHeight * (TotalImages);
                imageRect.anchoredPosition = new Vector2(imageRect.anchoredPosition.x, newY);
            }
        }
    }
    #endregion


    #region 其余函数
    //用于老虎机结束旋转后重置容器和图片的坐标
    private void ResetImagePositions()
    {
        SlotMachineContainer.anchoredPosition = m_InitialContainerPos;      //重置容器的坐标

        //重置各个图片的坐标
        for (int i = 0; i < SlotMachineContainer.childCount; i++)
        {
            RectTransform imageRect = SlotMachineContainer.GetChild(i).GetComponent<RectTransform>();

            imageRect.anchoredPosition = m_ImageArray[i];
        }
    }


    //更改目标图片的颜色
    private void ChangeTargetImageColor(Color thisColor)
    {
        Image targetImage = SlotMachineContainer.GetChild(TargetIndex).GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogError("Cannot get the Image component in the " + gameObject.name);
            return;
        }


        targetImage.color = thisColor;
    }


    //更改目标文本
    private void ChangeTargetText(string phraseKey)
    {
        TextMeshProUGUI targetText = SlotMachineContainer.GetChild(TargetIndex).GetComponentInChildren<TextMeshProUGUI>();
        if (targetText == null)
        {
            Debug.LogError("Cannot get the TextMeshProUGUI component in the children of " + gameObject.name);
            return;
        }



        if (LeanLocalization.CurrentLanguages != null)
        {
            targetText.text = LeanLocalization.GetTranslationText(phraseKey);   //根据当前语言赋值文本
        }
    }
    #endregion
}