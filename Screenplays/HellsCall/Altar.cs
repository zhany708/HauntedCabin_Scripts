using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using ZhangYu.Utilities;



public class Altar : MonoBehaviour     //放在仪式台上的脚本
{
    public GameObject EnemyPrefab;      //敌人的预制件
    public Core Core { get; private set; }


    //检查m_Combat是否为空，不是的话则返回它，是的话则调用GetCoreComponent函数以获取组件
    public Combat Combat => m_Combat ? m_Combat : Core.GetCoreComponent(ref m_Combat);
    private Combat m_Combat;

    public Stats Stats => m_Stats ? m_Stats : Core.GetCoreComponent(ref m_Stats);
    private Stats m_Stats;



    Coroutine m_EnemySpawnCoroutine;    //敌人生成的协程

    Timer m_DurationTimer;      //用于计时仪式时长的计时器

    [SerializeField] float m_RitualMaxHealth = 0f;     //仪式台的生命值上限
    [SerializeField] float m_HitResistance = 99f;      //仪式台的受击抗性

    float m_RitualDuration = 9f;        //仪式时间
    float m_EnemySpawnInterval = 3f;    //敌人生成的冷却
    float m_RestoreHealthAmout = 0f;    //玩家完成仪式后恢复的生命值

    bool m_IsHit = false;




    #region Unity内部函数
    private void Awake()
    {
        //初始化Core组件
        Core = GetComponentInChildren<Core>();      //从子物体那调用Core脚本
        Core.SetParameters(m_RitualMaxHealth, 0, m_HitResistance);   //将参数传给Core

        //初始化计数器
        m_DurationTimer = new Timer(m_RitualDuration);
    }

    private void Update()
    {
        if (Combat.IsHit && !m_IsHit)    //检查是否受到攻击，且当前是否处于受击状态
        {
            //将祷告石受击时的逻辑放在这
            HitLogic();
        }
    }

    private void OnEnable()
    {
        m_DurationTimer.OnTimerDone += FinishRitual;          //计时结束后进行仪式结束的逻辑
        Stats.OnHealthZero += GameOver;
    }

    private void OnDisable()
    {
        m_DurationTimer.OnTimerDone -= FinishRitual;
        Stats.OnHealthZero -= GameOver;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //只有当玩家拿到祷告石后，才允许玩家开始仪式
            if (HellsCall.Instance.GetCanStartRitual() )
            {
                SetAnimatorStart();     //设置动画器参数，以开始仪式
            }           
        }
    }
    #endregion


    #region 仪式相关
    private void SetAnimatorStart()      //先调用这个函数，随后才会在动画中调用开始仪式的函数
    {
        //仪式台动画：先是符号开始发亮，随后仪式台周围的光波会开始循环的环绕仪式台（无需设置环绕参数为true，因为发光动画结束后会直接过渡到环绕动画）
        Core.Animator.SetBool("Glow", true);    //播放符号发光动画
    }

    private void GenerateEnemyThroughTime(float duration, float spawnInterval)      //仪式期间持续生成敌人
    {
        StartCoroutine(m_DurationTimer.WaitForDuration() );     //开始仪式计时

        //开始生成敌人
        m_EnemySpawnCoroutine = StartCoroutine(EnemySpawnCoroutine(m_EnemySpawnInterval) );
    }

    private IEnumerator EnemySpawnCoroutine(float spawnInterval)        //敌人生成的协程
    {
        while (true)            //反复的生成敌人
        {
            GenerateSingleEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void GenerateSingleEnemy()      //生成单个敌人
    {
        //在房间内的随机位置生成敌人
        EnvironmentManager.Instance.GenerateEnemy(HellsCall.Instance.RitualRoomDoorController, EnemyPrefab);
    }




    private void HitLogic()      //受击相关的逻辑
    {
        Core.Animator.SetBool("Hit", true);     //需要做的：决定要不要画祷告石的受击动画

        m_IsHit = true;
    }



    private void FinishRitual()   //仪式结束后的逻辑
    {
        Core.Animator.SetBool("Circle", false);      //将环绕参数设置为false，以结束仪式台的环绕

        //停止敌人生成的循环
        if (m_EnemySpawnCoroutine != null)
        {
            StopCoroutine(m_EnemySpawnCoroutine);
        }

        HellsCall.Instance.PlayerStats.IncreaseHealth(m_RestoreHealthAmout);      //给玩家增加一定的血量
        HellsCall.Instance.IncrementRitualCount();            //增加仪式完成的计数
    }


    private void GameOver()     //跟Stats状态函数里的事件绑定在一起，或者放在仪式台死亡动画里
    {
        //需要做的：打开剧本失败面板
    }
    #endregion


    #region 动画帧事件
    private void StartRitual()      //放在仪式台发亮的那一帧
    {
        HellsCall.Instance.SetCanStartRitual(false);        //仪式开始后将布尔设置为false，防止玩家反复开始仪式

        //在一定时间内，定期在房间内生成敌人（生成位置随机）
        GenerateEnemyThroughTime(m_RitualDuration, m_EnemySpawnInterval);
    }
    
    private void SetAnimatorBool()
    {
        Core.Animator.SetBool("Hit", false);    //放在受击动画的最后几帧

        m_IsHit = false;
    }
    #endregion
}