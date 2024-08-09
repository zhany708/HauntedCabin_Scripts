using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;


/*
 * Introduction：用于多目标区域的QTE界面
 * Creator：Zhang Yu
*/

public class QTEPanel : BasePanel
{
    public List<Action> OnAllQTESuccessed = new List<Action>();         //接收方为需要进行QTE的所有脚本，用于为不同的区域做不同的结果逻辑
    public Action OnQTEFailed;                                          //接收方为需要进行QTE的所有脚本


    public static QTEPanel Instance { get; private set; }


    [SerializeField] List<RectTransform> m_AllTargetZones;      //界面内的所有目标区域
    [SerializeField] List<float> m_AllTargetZonePercent;        //所有目标区域对应的判定成功的角度的百分比（让不同的选择的难度不同）
    [SerializeField] RectTransform m_Needle;                    //围绕圆环旋转的指针
    [SerializeField] TextMeshProUGUI m_TipText;                 //给与玩家不同区域的结果提示的文本
    [SerializeField] float m_NeedleSpeed = 160f;                //指针旋转的速度
    [SerializeField] float m_ThresholdPerValue = 2f;            //每1点玩家属性值对应的判定成功角度（需要乘以2以得到最终角度）


    //用于储存需要的部分目标区域以及对应的判定成功的角度
    public Dictionary<RectTransform, float> TargetZoneDict { get; private set; } = new Dictionary<RectTransform, float>();

    //用于给予玩家不同区域的结果相关的提示（如“影响神志”）
    Dictionary<Color, string> m_ZoneEffects;


    //目标区域的原始角度（与编辑器中物体的角度可能会有出入，因为Unity会标准化角度至[-180, 180]的范围）
    List<int> m_AllTargetZoneRawRotation = new List<int>();
    List<float> m_TargetZoneSuccessThreshold = new List<float>();         //储存所有区域的判定成功的角度的列表，用于随机生成角度和坐标

    string m_FailTipText = "Default fail result";           //QTE失败的结果相关的提示
    int m_NumberOfZones;                                    //需要激活的目标区域的数量
    bool m_IsQTEActive = false;                             //表示QTE检查是否正在运行
    bool m_HasPassedTargetZone = false;                     //表示指针是否经过并超出了检查范围
    bool m_DoCheckPassTargetZone = false;                   //表示是否需要让指针只走一圈就自动失败
    float m_NeedleRotation = 0f;                            //指针的角度
    float m_Radius;                                         //圆环的半径，在编辑器里通过坐标系统得出

    [SerializeField] int m_MinRandomDegrees = -340;         //计算目标区域的随机角度时允许的最小值
    [SerializeField] int m_MaxRandomDegrees = -45;          //计算目标区域的随机角度时允许的最大值



    //以下变量用于测试，后续需要删除
    [SerializeField] Button m_TestButton;                   //测试按钮，用于开始QTE


    



    #region Unity内部函数
    protected override void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }


        CheckComponents();              //检查界面组件是否全部配置
        InitializeRadius();             //初始化圆环的半径
        InitializeZoneEffects();        //初始化提示字典
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



        m_TestButton.onClick.AddListener(() => StartQTE());


        /*模拟触发事件前调用的函数
        SetNumberOfZones(4);            //总共需要4个目标区域
        InitalizeTargetZones(5);        //模拟角色的属性值为5
        */
    }

    private void Update()
    {
        if (m_IsQTEActive)
        {
            UpdateNeedleRotation();             //持续更新指针角度

            if (m_DoCheckPassTargetZone)
            {
                HandleAutoFailOnPass();         //让指针转一圈后结果自动失败
            }


            //玩家按下空格时
            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckQTEResultForAllZones();
            }
        }
    }
    #endregion


    #region 主要函数
    //开始旋转指针
    public void StartQTE()
    {
        SetRandomPositionAndRotationForTargetZones();       //随机设置目标区域的坐标


        //进行QTE检查前重置指针旋转的值
        m_HasPassedTargetZone = false;
        m_NeedleRotation = 0f;
        m_Needle.localRotation = Quaternion.Euler(0, 0, 0);


        m_IsQTEActive = true;       //设置布尔后，才会真正开始旋转

        //UpdateTipText();            //更新提示文本信息
    }


    private void UpdateNeedleRotation()
    {
        //持续更新指针的角度（指针的坐标应跟圆盘一致，且半径也一致。这样就只需要更改角度即可实现“运动”效果）
        m_NeedleRotation -= m_NeedleSpeed * Time.deltaTime;
        m_Needle.localRotation = Quaternion.Euler(0, 0, m_NeedleRotation);
    }

    private void HandleAutoFailOnPass()
    {
        //检查指针是否经过最后一个目标区域（需要额外减去该目标区域的判定区域的一半）
        if (!m_HasPassedTargetZone && m_NeedleRotation <= GetSmallestRawRotation() - TargetZoneDict[m_AllTargetZones[GetIndexOfSmallestRawRotation()]])
        {
            m_HasPassedTargetZone = true;
        }

        //当指针超出范围后玩家依然没有反应时
        if (m_HasPassedTargetZone)
        {
            m_IsQTEActive = false;

            FailLogic();        //执行失败相关的逻辑
        }      
    }




    //逐个检查指针是否处于某个目标区域内
    private void CheckQTEResultForAllZones()
    {
        m_IsQTEActive = false;

        for (int i = 0; i < m_NumberOfZones; i++)
        {
            //检查指针和目标区域之间的角度偏差
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(m_NeedleRotation, m_AllTargetZoneRawRotation[i]));


            if (angleDifference <= TargetZoneDict[m_AllTargetZones[i]])
            {
                SuccessLogic(i);       //进行该区域的成功逻辑
                return;
            }
        }


        //指针不处于任何区域时
        FailLogic();       
    }



    //QTE成功相关的逻辑
    private void SuccessLogic(int zoneIndex)
    {
        Debug.Log($"QTE Success on Zone {zoneIndex}!");

        //根据结果回调具体的事件
        OnAllQTESuccessed[zoneIndex]?.Invoke();

        CompleteLogic();
    }

    //QTE失败相关的逻辑
    private void FailLogic()
    {
        Debug.Log("QTE Failed!");

        OnQTEFailed?.Invoke();      //回调QTE失败相关的逻辑

        CompleteLogic();
    }

    //QTE检测结束时需要执行的逻辑（无论成功还是失败）
    private void CompleteLogic()
    {
        ClearAllSubscriptions();        //重置事件绑定的函数


        //延迟1秒后再淡出界面
        Coroutine delayFadeOut = StartCoroutine(Delay.Instance.DelaySomeTime(1f, () =>
        {
            Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);       //淡出界面   
        }));

        generatedCoroutines.Add(delayFadeOut);     //将协程加进列表
    }
    #endregion


    #region 其余函数
    //将所有区域的判定成功的区域储存进列表，用于随机生成区域的角度和坐标
    private void StoreAllSuccessThreshold()
    {
        m_TargetZoneSuccessThreshold.Clear();     //赋值前，先清空储存判定成功的角度的链表（否则会越积越多）


        for (int i = 0; i < m_NumberOfZones; i++)
        {
            //从字典中获取判定成功的角度
            m_TargetZoneSuccessThreshold.Add(TargetZoneDict[m_AllTargetZones[i]]);
        }
    }



    //随机设置所有目标区域的坐标（防止每次QTE检验时，目标区域都在同一位置）
    private void SetRandomPositionAndRotationForTargetZones()
    {
        //将生成完的区域的随机角度储存进链表
        List<int> allRotations = GenerateUniqueRandomNumbers(m_NumberOfZones, m_MinRandomDegrees, m_MaxRandomDegrees, m_TargetZoneSuccessThreshold);

        m_AllTargetZoneRawRotation.Clear();     //赋值前，先清空储存原始角度的链表（否则会越积越多）


        for (int i = 0; i < m_NumberOfZones; i++)
        {
            //根据角度计算应该处于的坐标（使用Cos函数时，参数需要从度数转换成弧度）  
            float Xpos = m_Radius * Mathf.Cos((allRotations[i] + 90f) * Mathf.Deg2Rad);
            float Ypos = m_Radius * Mathf.Sin((allRotations[i] + 90f) * Mathf.Deg2Rad);


            m_AllTargetZones[i].anchoredPosition = new Vector2(Xpos, Ypos);                     //设置坐标
            m_AllTargetZones[i].localEulerAngles = new Vector3(0, 0, allRotations[i]);          //设置角度


            m_AllTargetZoneRawRotation.Add(allRotations[i]);       //赋值原始角度
        }
    }


    //第一个参数为需要生成的数量，第二和第三个参数为可生成的最小和最大数，第四个参数为每个数对应的距离其它数之间的最小值（也就是判定角度）
    List<int> GenerateUniqueRandomNumbers(int neededNum, int minNum, int maxNum, List<float> minDistances)
    {
        List<int> allNumbers = new List<int>();


        //只要链表没有储存到足够的数时，就持续进行循环
        while (allNumbers.Count < neededNum)
        {
            int newNumber = UnityEngine.Random.Range(minNum, maxNum);       //在范围内随机生成数字
            bool isTooClose = false;
            float currentMinDistance = minDistances[allNumbers.Count] * 2;    //获取当前索引的数字所能接受的距离其它数的最小值


            //只有在链表有数的情况下才检查
            if (allNumbers.Count > 0)
            { 
                foreach (int number in allNumbers)
                {
                    int targetNumberIndex = allNumbers.IndexOf(number);               //获取正在比较的数所在的索引
                    float targetMinDistance = minDistances[targetNumberIndex] * 2;    //获取正在比较的数字所能接受的距离其它数的最小值


                    //检查新生成的数是否跟任何其它数过于接近，既要检查新生成的数的最小距离，也要检查正在比较的数的最小距离
                    //（如果是0-360的封闭圆环，则需要考虑最小数和最大数之间的距离。这里不是封闭的，因此不考虑）
                    if (Mathf.Abs(newNumber - number) < currentMinDistance || 
                        Mathf.Abs(newNumber - number) < targetMinDistance /*||
                        Mathf.Abs(newNumber - number) > (Mathf.Abs(maxNum - minNum) - currentMinDistance)*/)
                    {
                        isTooClose = true;
                        break;
                    }
                }
            }

            //只有当新生成的数跟所有其他数都较远时才加进链表
            if (!isTooClose)
            {
                allNumbers.Add(newNumber);
            }
        }

        return allNumbers;
    }


   
    //根据判定成功的角度更改目标区域的宽度
    private void SetTargetZoneWidth()
    {
        //圆环的周长
        float circumference = 2 * Mathf.PI * m_Radius;


        for (int i = 0; i < m_NumberOfZones; i++)
        {
            //目标区域的宽度
            float targetZoneWidth = (TargetZoneDict[m_AllTargetZones[i] ] * 2 / 360f) * circumference;

            //设置目标区域的宽度
            m_AllTargetZones[i].sizeDelta = new Vector2(targetZoneWidth, m_AllTargetZones[i].sizeDelta.y);
        }    
    }


    protected override void HandleFadeOutFinished()
    {
        base.HandleFadeOutFinished();

        m_Needle.localRotation = Quaternion.Euler(0, 0, 0);         //重置指针的角度
    }



    //删除所有回调事件绑定的函数
    public void ClearAllSubscriptions()
    {
        OnAllQTESuccessed.Clear();      //清空链表
        OnQTEFailed = null;             //重置回调事件
    }
    #endregion


    #region 初始化相关
    private void CheckComponents()
    {
        if (m_Needle == null || m_AllTargetZones == null || m_AllTargetZonePercent == null || m_NeedleSpeed <= 0 || m_ThresholdPerValue <= 0)
        {
            Debug.LogError("Some components are not assigned in the " + gameObject.name);
            return;
        }
    }

    private void InitializeRadius()
    {
        //计算圆环的半径（这里由于圆并不完全填充由长和宽组成的正方形，因此不能通过此方法得出半径）
        m_Radius = (m_AllTargetZones[0].parent as RectTransform).rect.width / 2f;
    }


    //根据玩家属性值调整所有QTE的判定成功的角度             需要做的：等游戏难易度系统设置好后，也需要考虑游戏难易度
    public void InitalizeTargetZones(float playerPropertyValue)
    {
        //根据需要的目标区域数量，从所有目标区域数组中获取
        for (int i = 0; i < m_NumberOfZones; i++)
        {
            m_AllTargetZones[i].gameObject.SetActive(true);     //激活需要的区域物体


            //根据玩家的属性数值以及选项的难易度百分比计算最终的判定成功的角度
            float zoneSuccessThreshold = m_ThresholdPerValue * playerPropertyValue * m_AllTargetZonePercent[i];


            if (!TargetZoneDict.ContainsKey(m_AllTargetZones[i]))
            {
                TargetZoneDict.Add(m_AllTargetZones[i], zoneSuccessThreshold);
            }
        }


        SetTargetZoneWidth();           //设置所有目标区域的宽度
        StoreAllSuccessThreshold();     //将所有区域的判定成功的区域储存进列表
    }
    #endregion


    #region 不同区域结果的提示文本相关
    //初始化给予玩家不同区域的结果相关的提示的字典
    private void InitializeZoneEffects()
    {
        m_ZoneEffects = new Dictionary<Color, string>
        {
            { Color.red, "affects sanity" },
            { Color.yellow, "affects strength" },
            { Color.blue, "affects speed" },
            { Color.green, "affects knoweledge" },
            { Color.magenta, "affects health" },
            { Color.cyan, "affects weapon" }
        };
    }

    
    private void UpdateTipText()
    {
        List<string> tipTexts = new List<string>();     //用于储存所需的所有文本的链表


        for (int i = 0; i < m_NumberOfZones; i++)
        {
            Image zoneImage = m_AllTargetZones[i].GetComponent<Image>();
            if (zoneImage == null)
            {
                Debug.LogError($"Cannot get the Image component from {m_AllTargetZones[i].gameObject.name}");
                return;
            }


            if (m_ZoneEffects.ContainsKey(zoneImage.color) )
            {
                string effectText = m_ZoneEffects[zoneImage.color];         //从字典那获取不同的颜色区域所对应的文本内容

                tipTexts.Add($"{GetColorName(zoneImage.color)} zone: {effectText}");
            }
        }

        tipTexts.Add($"Failing the QTE: {m_FailTipText}");      //将失败的提示文本加进链表，最后显示出来

        m_TipText.text = string.Join("\n", tipTexts);           //将链表中的文本显示出来，每段文本之间空一行
    }

    private string GetColorName(Color color)
    {
        if (color == Color.red) return "Red";
        if (color == Color.yellow) return "Yellow";
        if (color == Color.blue) return "Blue";
        if (color == Color.green) return "Green";
        if (color == Color.magenta) return "Magenta";
        if (color == Color.cyan) return "Cyan";

        return "UnKnown";       //颜色都不匹配的时候，返回UnKnown
    }


    //用于外部调用，以赋值提示的文本给不同的区域
    public void SetZoneEffect(Color zoneColor, string effectText)
    {
        //如果字典中有参数内的颜色，则更改文本
        if (m_ZoneEffects.ContainsKey(zoneColor) )
        {
            m_ZoneEffects[zoneColor] = effectText;
        }

        //如果没有，则将颜色和文本加进字典
        else
        {
            m_ZoneEffects.Add(zoneColor, effectText);
        }
    }


    //用于外部调用，以赋值提示的文本给失败时的结果
    public void SetFailTipText(string failTip)
    {
        m_FailTipText = failTip;
    }
    #endregion


    #region Getters
    //获取所有区域中角度最小的值
    private int GetSmallestRawRotation()
    {
        return m_AllTargetZoneRawRotation.Min();
    }

    //获取列表中储存最小角度值的索引
    private int GetIndexOfSmallestRawRotation()
    {
        return m_AllTargetZoneRawRotation.IndexOf(GetSmallestRawRotation());
    }
    #endregion


    #region Setters
    //设置需要激活的目标区域的数量
    public void SetNumberOfZones(int thisNumber)
    {
        //检查参数是否小于等于0
        if (thisNumber <= 0)
        {
            Debug.LogError($"Cannot assign {thisNumber} target zones in the " + gameObject.name);
            return;
        }


        m_NumberOfZones = thisNumber;
        m_DoCheckPassTargetZone = m_NumberOfZones <= 1;     //只有当目标区域小于等于1时，才只让指针转一圈      
    }

    public void SetSuccessedAction(List<Action> thisAction)
    {
        OnAllQTESuccessed = thisAction;     //赋值成功相关的回调事件
    }

    public void SetFailedAction(Action thisAction)
    {
        OnQTEFailed = thisAction;           //赋值失败相关的回调事件
    }
    #endregion
}