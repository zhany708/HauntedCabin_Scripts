using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;



//用于管理QTE相关的逻辑         需要做的：想出一个表示不同结果的区域的逻辑
public class QTEPanel : BasePanel
{
    public event Action OnQTESuccessed;         //接收方为需要进行QTE的所有脚本（比如部分事件）


    [SerializeField] RectTransform m_Needle;                //围绕圆环旋转的指针
    [SerializeField] RectTransform m_TargetZone;            //指针需要停留的目标区域
    [SerializeField] float m_NeedleSpeed = 180f;             //指针旋转的速度
    [SerializeField] float m_SuccessThreshold = 15f;        //目标区域前后的判定成功的角度，用于QTE检查（难度越大，此变量越小）
    [SerializeField] float m_ThresholdPerValue = 4f;        //每1点玩家属性值对应的判定成功角度（需要乘以2）



    bool m_IsQTEActive = false;                 //表示QTE检查是否正在运行
    bool m_HasPassedTargetZone = false;         //表示指针是否经过并超出了检查范围
    float m_NeedleRotation = 0f;                //指针的角度
    int m_TargetZoneRawRotation;              //目标区域的原始角度（与编辑器中物体的角度可能会有出入，因为Unity会标准化角度至[-180， 180]的范围）

    float m_Radius;                             //圆环的半径，在编辑器里通过坐标系统得出的
    [SerializeField] int m_MinRandomDegrees = -355;          //计算目标区域的随机角度时允许的最小值
    [SerializeField] int m_MaxRandomDegrees = -30;           //计算目标区域的随机角度时允许的最大值


    //以下变量用于测试，后续需要删除
    [SerializeField] Button m_TestButton;               //测试按钮，用于开始QTE


    [SerializeField] TextMeshProUGUI m_CountText;       //用于成功和失败的计数文本
    string m_InitialCountText;                          //计数文本的初始文本，用于后续更新
    int m_SuccessCount = 0;
    int m_FailCount = 0;








    #region Unity内部函数
    protected override void Awake()
    {
        if (m_Needle == null || m_TargetZone == null || m_NeedleSpeed <= 0 || m_SuccessThreshold <= 0)
        {
            Debug.LogError("Some components are not assigned in the " + gameObject.name);
            return;
        }


        //计算圆环的半径（这里由于圆并不完全填充由长和宽组成的正方形，因此不能通过此方法得出半径）
        m_Radius = (m_TargetZone.parent as RectTransform).rect.width / 2f;

        //设置目标区域的宽度
        SetTargetZoneWidth();
    }

    private void Start()
    {
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.QTEPanel;
        }

        if (!UIManager.Instance.NoMoveAndAttackList.Contains(this))
        {
            UIManager.Instance.NoMoveAndAttackList.Add(this);       //界面淡入后禁止玩家移动和攻击
        }


        //StartQTE();     //开始测试


        m_TestButton.onClick.AddListener(() => StartQTE());



        m_InitialCountText = m_CountText.text;      //初始化成功计数的文本

        UpdateCountText();      //初始化成功计数
    }

    private void Update()
    {
        if (m_IsQTEActive)
        {
            //持续更新指针的角度（指针的坐标应跟圆盘一致，且半径也一致。这样就只需要更改角度即可实现“运动”效果）
            m_NeedleRotation -= m_NeedleSpeed * Time.deltaTime;
            m_Needle.localRotation = Quaternion.Euler(0, 0, m_NeedleRotation);


            //检查指针是否经过目标区域（需要额外减去判定区域的一半）
            if (!m_HasPassedTargetZone && m_NeedleRotation <= m_TargetZoneRawRotation - m_SuccessThreshold)
            {
                m_HasPassedTargetZone = true;
            }

            //当指针超出范围后玩家依然没有反应时
            if (m_HasPassedTargetZone)
            {
                m_IsQTEActive = false;

                FailLogic();        //执行失败相关的逻辑

                return;             //返回，以避免执行非必要的其他逻辑
            }



            //玩家按下空格时
            if (Input.GetKeyDown(KeyCode.Space) )
            {
                CheckQTEResult();
                return;
            }
        }
    }
    #endregion


    #region 主要函数
    //开始旋转指针
    public void StartQTE()
    {
        SetRandomPositionAndRotationForTargetZone();       //随机设置目标区域的坐标

        

        //进行QTE检查前重置指针旋转的值
        m_HasPassedTargetZone = false;
        m_NeedleRotation = 0f;
        m_Needle.localRotation = Quaternion.Euler(0, 0, 0);


        m_IsQTEActive = true;       //设置布尔后，才会真正开始旋转
    }

    private void CheckQTEResult()
    {
        m_IsQTEActive = false;

        //检查指针和目标区域之间的角度偏差
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(m_NeedleRotation, m_TargetZoneRawRotation));


        if (angleDifference <= m_SuccessThreshold)
        {
            SuccessLogic();
        }

        else
        {
            FailLogic();
        }
    }



    //QTE成功相关的逻辑
    private void SuccessLogic()
    {
        m_SuccessCount++;

        //Debug.Log("QTE Success!");

        OnQTESuccessed?.Invoke();           //回调事件

        CompleteLogic();
    }

    //QTE失败相关的逻辑
    private void FailLogic()
    {
        m_FailCount++;

        //Debug.Log("QTE Failed!");

        CompleteLogic();
    }

    //QTE检测结束时需要执行的逻辑（无论成功还是失败）
    private void CompleteLogic()
    {
        ClearAllSubscriptions();        //重置事件绑定的函数

        //Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);       //淡出界面

        UpdateCountText();      //更新成功计数
    }
    #endregion


    #region 其余函数
    //为目标区域设置随机角度和坐标
    private void SetRandomPositionAndRotationForTargetZone()
    {
        //从允许的最小值开始，否则过于靠前会导致玩家来不及反应
        int randomRotation = UnityEngine.Random.Range(m_MinRandomDegrees, m_MaxRandomDegrees);


        //根据角度计算应该处于的坐标（使用Cos函数时，参数需要从度数转换成弧度）  
        float Xpos = m_Radius * Mathf.Cos( (randomRotation + 90f) * Mathf.Deg2Rad);
        float Ypos = m_Radius * Mathf.Sin( (randomRotation + 90f) * Mathf.Deg2Rad);


        m_TargetZone.anchoredPosition = new Vector2(Xpos, Ypos);                    //设置坐标
        m_TargetZone.localEulerAngles = new Vector3(0, 0, randomRotation);          //设置角度


        m_TargetZoneRawRotation = randomRotation;       //赋值原始角度
    }


    //删除所有回调事件绑定的函数
    public void ClearAllSubscriptions()         
    {
        OnQTESuccessed = null;
    }


    //根据判定成功的角度更改目标区域的宽度
    private void SetTargetZoneWidth()
    {
        //圆环的周长
        float circumference = 2 * Mathf.PI * m_Radius;

        //目标区域的宽度
        float targetZoneWidth = (m_SuccessThreshold * 2 / 360f) * circumference;

        //设置目标区域的宽度
        m_TargetZone.sizeDelta = new Vector2(targetZoneWidth, m_TargetZone.sizeDelta.y);
    }


    //根据玩家属性值调整QTE的难易度             需要做的：等游戏难易度系统设置好后，也需要考虑游戏难易度
    public void ChangeSuccessThreshold(float playerPropertyValue)
    {
        m_SuccessThreshold = playerPropertyValue * m_ThresholdPerValue;

        SetTargetZoneWidth();       //设置完判定成功的角度后更改目标区域的宽度
    }




    //更新成功和失败计数
    private void UpdateCountText()
    {
        m_CountText.text = string.Format(m_InitialCountText, m_SuccessCount, m_FailCount);
    }
    #endregion
}