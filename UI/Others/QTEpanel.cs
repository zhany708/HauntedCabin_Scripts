using UnityEngine;
using UnityEngine.UI;



//用于管理QTE相关的逻辑
public class QTEpanel : BasePanel
{
    public event Action OnQTESuccessed;         //接收方为需要进行QTE的所有脚本（比如部分事件）


    [SerializeField] RectTransform m_Needle;              //围绕圆环旋转的指针
    [SerializeField] RectTransform m_TargetZone;          //指针需要停留的目标区域
    [SerializeField] float m_NeedleSpeed = 200f;          //指针旋转的速度
    [SerializeField] float m_SuccessThreshold = 15f;      //目标区域前后的判定成功的角度，用于QTE检查（难度越大，此变量越小）




    bool m_IsQTEActive = false;                 //表示QTE检查是否正在运行
    bool m_HasPassedTargetZone = false;         //表示指针是否经过并超出了检查范围
    float m_NeedleRotation = 0f;                //指针的角度
    float m_TargetZoneRotation;                 //目标区域的角度

    float m_Radius;                             //圆环的半径





    #region Unity内部函数
    protected override void Awake()
    {
        if (m_Needle == null || m_TargetZone == null || m_NeedleSpeed <= 0 || m_SuccessThreshold <= 0)
        {
            Debug.LogError("Some components are not assigned in the " + gameObject.name);
            return;
        }


        //计算圆环的半径
        m_Radius = (m_TargetZone.parent as RectTransform).rect.width / 2f;

        //设置目标区域的宽度
        SetTargetZoneWidth();
    }

    private void Update()
    {
        if (m_IsQTEActive)
        {
            //持续更新指针的角度
            m_NeedleRotation += m_NeedleSpeed * Time.deltaTime;
            m_Needle.localRotation = Quaternion.Euler(0, 0, -m_NeedleRotation);


            //检查指针是否经过目标区域
            if (!m_HasPassedTargetZone && m_NeedleRotation >= m_TargetZoneRotation)
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
        m_TargetZoneRotation = m_TargetZone.localRotation.eulerAngles.z;        //计算目标区域的角度

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
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(m_NeedleRotation, m_TargetZoneRotation));


        if (angleDifference <= m_SuccessThreshold)
        {
            SuccessLogic();
        }

        else
        {
            FailLogic();
        }
    }
    #endregion


    #region 其余函数
    //QTE成功相关的逻辑
    private void SuccessLogic()
    {
        Debug.Log("QTE Success!");

        OnQTESuccessed?.Invoke();           //回调事件

        CompleteLogic();
    }

    //QTE失败相关的逻辑
    private void FailLogic()
    {
        Debug.Log("QTE Failed!");

        CompleteLogic();
    }

    //QTE检测结束时需要执行的逻辑（无论成功还是失败）
    private void CompleteLogic()
    {
        ClearAllSubscriptions();        //重置事件绑定的函数

        Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);       //淡出界面
    }


    //删除所有事件绑定的函数
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
        float targetZoneWidth = (m_SuccessThreshold / 360f) * circumference;

        //设置目标区域的宽度
        m_TargetZone.sizeDelta = new Vector2(targetZoneWidth, m_TargetZone.sizeDelta.y);
    }
    #endregion
}