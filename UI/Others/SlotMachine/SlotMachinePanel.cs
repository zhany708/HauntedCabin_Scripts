using UnityEngine;
using DG.Tweening;
using TMPro;



//用于老虎机界面
public class SlotMachinePanel : BasePanel
{
    [SerializeField] RectTransform m_RoomContainer;                 //储存房间图片的容器
    [SerializeField] RectTransform m_EventContainer;                //储存事件图片的容器
    [SerializeField] RectTransform m_ScreenplayBackground;          //显示触发的剧本的界面
    [SerializeField] TextMeshProUGUI m_TipText;                     //提示文本
    [SerializeField] Animator m_PanelBackgroundAnimator;            //界面背景的动画控制器
    [SerializeField] Animator[] m_ResultFlowAnimators;              //流向剧本界面的动画控制器


    Vector2[] m_RoomImageArray;                     //储存所有房间图片的坐标的数组，用于Debug
    Vector2[] m_EventImageArray;                    //储存所有事件图片的坐标的数组

    Vector2 m_InitialRoomContainerPos;              //房间容器的初始位置
    Vector2 m_InitialEventContainerPos;             //事件容器的初始位置
    Sequence m_ScrollSequence;                      //老虎机的动画包含的所有逻辑

    string m_RoomTextForChanging;                   //老虎机开始旋转后用于更改房间的文本
    string m_EventTextForChanging;                  //老虎机开始旋转后用于更改事件的文本
    string m_ScreenplayTextToDisplay;               //老虎机旋转结束后显示的剧本名
    bool m_IsScrolling = false;                     //表示老虎机是否正在旋转
    float m_ImageHeight;                            //每个图片的高
    float m_ScrollDuration = 10f;                   //一次旋转的时长
    float m_ScrollSpeed = 10f;                      //老虎机滚动的速度
    int m_TotalImages = 11;                         //图片的总数
    int m_TargetIndex = 5;                          //最终停留的图片的索引
    
    






    #region Unity内部函数
    protected override void Awake()
    {
        base.Awake();


        if (m_RoomContainer == null || m_EventContainer == null || m_ScreenplayBackground == null || m_TipText == null || m_PanelBackgroundAnimator == null || m_ResultFlowAnimators == null)
        {
            Debug.LogError($"Some components are not assigned in the {gameObject.name}");
            return;
        }

        //初始化所有数组
        m_RoomImageArray = new Vector2[m_TotalImages];
        m_EventImageArray = new Vector2[m_TotalImages];

        //初始化所有容器的坐标
        m_InitialRoomContainerPos = m_RoomContainer.anchoredPosition;       
        m_InitialEventContainerPos = m_EventContainer.anchoredPosition;
    }

    private void OnEnable()
    {
        SetBothMoveableAndAttackable(false);        //界面打开时禁止玩家移动和攻击

        OnFadeInFinished += DelayStartSlotMachine;       //界面淡入后开始老虎机旋转
        OnFadeOutFinished += ClosePanel;
    }

    private void Start()
    {
        //计算每个图片的高度
        m_ImageHeight = m_RoomContainer.GetChild(0).GetComponent<RectTransform>().rect.height;
    }

    private void Update()
    {
        //按空格开始旋转（用于测试，待界面完善后删除这部分）
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSlotMachine();
        }
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();

        SetBothMoveableAndAttackable(true);         //恢复玩家的移动和攻击

        OnFadeInFinished -= DelayStartSlotMachine;
        OnFadeOutFinished -= ClosePanel;
    }
    #endregion


    #region 主要函数
    private void StartSlotMachine()
    {
        if (m_IsScrolling) return;


        //记录所有房间图片的初始坐标
        for (int i = 0; i < m_RoomContainer.childCount; i++)
        {
            RectTransform roomImageRect = m_RoomContainer.GetChild(i).GetComponent<RectTransform>();

            m_RoomImageArray[i] = roomImageRect.anchoredPosition;
        }

        //记录所有事件图片的初始坐标
        for (int i = 0; i < m_EventContainer.childCount; i++)
        {
            RectTransform eventImageRect = m_EventContainer.GetChild(i).GetComponent<RectTransform>();

            m_EventImageArray[i] = eventImageRect.anchoredPosition;
        }

        //Debug.Log($"Before: The position of target image is {m_ImageArray[TargetIndex]} and the position of Container is {SlotMachineContainer.anchoredPosition}");

        m_IsScrolling = true;

        ScrollContinuously();
    }

    private void DelayStartSlotMachine()
    {
        //等待1秒后，开始老虎机效果
        Coroutine startSlotMachineCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(1f, () => StartSlotMachine() ) );

        generatedCoroutines.Add(startSlotMachineCoroutine);            //将协程加进列表
    }

    //用于老虎机的滚动
    private void ScrollContinuously()
    {
        //计算老虎机一次旋转的总长度（如果图片间距大于0的话，这里要进行改动，否则会导致旋转结束后图片位置与初始位置不一致）
        float totalDistance = m_ImageHeight * m_TotalImages * m_ScrollSpeed;


        //创建循环动画以进行老虎机旋转
        m_ScrollSequence = DOTween.Sequence();

        //先进行房间的旋转，再进行事件的。Ease.Linear的速度是全程均匀的，而Ease.OutCubic的速度是在末尾逐渐下降的
        m_ScrollSequence.Append(m_RoomContainer.DOAnchorPosY(m_RoomContainer.anchoredPosition.y - totalDistance, m_ScrollDuration).SetEase(Ease.OutCubic))
                //第一个动画结束后，第二个动画开始前执行的逻辑
                .AppendCallback(() =>
                {
                    m_PanelBackgroundAnimator.SetBool("IsUpLightOn", true);     //让背景界面上方的灯亮

                    ChangeScreenplayText();         //更改即将显示的剧本名
                })
            
                
                //Append是在上一个效果结束后执行，Join是同时执行
                .Append(m_EventContainer.DOAnchorPosY(m_EventContainer.anchoredPosition.y - totalDistance, m_ScrollDuration).SetEase(Ease.OutCubic))

                //Sequence期间持续执行的逻辑
                .OnUpdate(() =>
                {
                    CheckAndRepositionImages();     //持续检查并更新图片的坐标


                    //不能开始旋转后就立刻更改，否则玩家会看到
                    if (m_RoomContainer.anchoredPosition.y <= -100)
                    {
                        ChangeTargetText(true);                         //更改房间的目标文本
                    }

                    if (m_EventContainer.anchoredPosition.y <= -100)
                    {
                        ChangeTargetText(false);                        //更改事件的目标文本
                    }
                })

                //Sequence结束后执行的逻辑
                .OnComplete(() =>
                {
                    HandleSequenceComplete();
                });
        
             
        m_ScrollSequence.Play();        //设置完动画后开始播放
    }
    



    //用于无限循环滚动图片
    private void CheckAndRepositionImages()
    {
        //循环滚动房间图片
        for (int i = 0; i < m_RoomContainer.childCount; i++)
        {
            RectTransform roomImageRect = m_RoomContainer.GetChild(i).GetComponent<RectTransform>();

            //由于老虎机的旋转是移动SlotMachineContainer，所以检查坐标时必须加上容器的坐标
            if (roomImageRect.anchoredPosition.y + m_RoomContainer.anchoredPosition.y < -m_ImageHeight * (m_TotalImages))
            {
                //将图片移至最上方
                float newY = roomImageRect.anchoredPosition.y + m_ImageHeight * (m_TotalImages);
                roomImageRect.anchoredPosition = new Vector2(roomImageRect.anchoredPosition.x, newY);
            }
        }

        //循环滚动事件图片
        for (int i = 0; i < m_EventContainer.childCount; i++)
        {
            RectTransform eventImageRect = m_EventContainer.GetChild(i).GetComponent<RectTransform>();

            if (eventImageRect.anchoredPosition.y + m_EventContainer.anchoredPosition.y < -m_ImageHeight * (m_TotalImages))
            {
                float newY = eventImageRect.anchoredPosition.y + m_ImageHeight * (m_TotalImages);
                eventImageRect.anchoredPosition = new Vector2(eventImageRect.anchoredPosition.x, newY);
            }
        }
    }



    public void StartTipTextAnimation()
    {
        //只有在提示文本未激活时才执行逻辑，防止重复进行协程
        if (!m_TipText.gameObject.activeSelf)
        {
            m_TipText.gameObject.SetActive(true);         //激活提示文本，提醒玩家按空格或点击以继续游戏

            //等待玩家按空格或点击鼠标
            Coroutine firstWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
            {
                m_TipText.gameObject.SetActive(false);                      //取消激活提示文本

                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);       //淡出界面
            }));

            generatedCoroutines.Add(firstWaitForInputCoroutine);            //将协程加进列表
        }      
    }


    //序列结束后执行的逻辑
    private void HandleSequenceComplete()
    {
        //设置动画器的布尔
        m_PanelBackgroundAnimator.SetBool("IsDownLightOn", true);       //让背景界面下方的灯亮

        for (int i = 0; i < m_ResultFlowAnimators.Length; i++)
        {
            m_ResultFlowAnimators[i].SetBool("IsFlow", true);    //让界面开始流动
        }


        m_IsScrolling = false;

        ResetImagePositions();      //重置坐标
               
        //Debug.Log($"After: The position of target image is {m_ImageArray[TargetIndex]} and the position of Container is {SlotMachineContainer.anchoredPosition}"); 
    }
    #endregion


    #region 其余函数
    //用于老虎机结束旋转后重置容器和图片的坐标
    private void ResetImagePositions()
    {
        m_RoomContainer.anchoredPosition = m_InitialRoomContainerPos;       //重置房间容器的坐标
        m_EventContainer.anchoredPosition = m_InitialEventContainerPos;     //重置事件容器的坐标

        //重置各个房间图片的坐标
        for (int i = 0; i < m_RoomContainer.childCount; i++)
        {
            RectTransform imageRect = m_RoomContainer.GetChild(i).GetComponent<RectTransform>();

            imageRect.anchoredPosition = m_RoomImageArray[i];
        }

        //重置各个事件图片的坐标
        for (int i = 0; i < m_EventContainer.childCount; i++)
        {
            RectTransform imageRect = m_EventContainer.GetChild(i).GetComponent<RectTransform>();

            imageRect.anchoredPosition = m_EventImageArray[i];
        }
    }



    //更改目标（房间，事件）文本
    private void ChangeTargetText(bool isRoom)
    {
        //根据参数中的布尔决定更改哪个容器
        RectTransform targetContainer = isRoom ? m_RoomContainer : m_EventContainer;

        TextMeshProUGUI targetText = targetContainer.GetChild(m_TargetIndex).GetComponentInChildren<TextMeshProUGUI>();
        if (targetText == null)
        {
            Debug.LogError($"Cannot get the TextMeshProUGUI component in the children of {targetContainer.gameObject.name}");
            return;
        }

        //根据参数中的布尔决定要赋值的文本
        string textForChanging = isRoom ? m_RoomTextForChanging : m_EventTextForChanging;


        if (textForChanging == null)
        {
            Debug.LogError($"The {textForChanging} is not assigned yet in the {gameObject.name}");
            return;
        }


        targetText.text = textForChanging;      //将翻译后的文本赋值过去
    }

    //更改剧本文本
    private void ChangeScreenplayText()
    {
        TextMeshProUGUI screenplayText = m_ScreenplayBackground.GetComponentInChildren<TextMeshProUGUI>();
        if (screenplayText == null)
        {
            Debug.LogError($"Cannot get the TextMeshProUGUI component in the children of {screenplayText.gameObject.name}");
            return;
        }


        if (m_ScreenplayTextToDisplay == null)
        {
            Debug.LogError($"The ScreenplayTextToDisplay variable is not assigned yet in the {gameObject.name}");
            return;
        }


        screenplayText.text = m_ScreenplayTextToDisplay;        //将翻译后的文本赋值过去
    }
    #endregion


    #region Setters
    public void SetTextForChanging(string thisRoomText, string thisEventText, string thisScreenplayText)
    {
        m_RoomTextForChanging = thisRoomText;
        m_EventTextForChanging = thisEventText;
        m_ScreenplayTextToDisplay = thisScreenplayText;
    }
    #endregion


    #region Getters
    public RectTransform GetScreenplayBackground()
    {
        return m_ScreenplayBackground;
    }
    #endregion
}