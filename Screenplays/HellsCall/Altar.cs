using System.Collections;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using ZhangYu.Utilities;



public class Altar : MonoBehaviour     //放在仪式台上的脚本
{
    public GameObject EnemyPrefab;      //敌人的预制件
    public Core Core { get; private set; }

    public Combat Combat
    {
        get
        {
            if (m_Combat) { return m_Combat; }
            m_Combat = Core.GetCoreComponent<Combat>();
            return m_Combat;
        }
    }
    private Combat m_Combat;



    Coroutine m_EnemySpawnCoroutine;    //敌人生成的协程

    Timer m_DurationTimer;      //用于计时仪式时长的计时器

    [SerializeField] float m_RitualMaxHealth = 0f;     //仪式台的生命值上限
    [SerializeField] float m_HitResistance = 99f;       //仪式台的受击抗性

    float m_RitualDuration = 9f;        //仪式时间
    float m_EnemySpawnInterval = 3f;    //敌人生成的冷却

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
            Core.Animator.SetBool("Hit", true);

            m_IsHit = true;
        }
    }

    private void OnEnable()
    {
        //将函数绑定到计时结束的事件
        m_DurationTimer.OnTimerDone += FinishRitual;          //计时结束后进行仪式结束的逻辑
    }

    private void OnDisable()
    {
        m_DurationTimer.OnTimerDone -= FinishRitual;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //只有当玩家拿到祷告石后，才允许玩家开始仪式
            if (HellsCall.Instance.GetCanStartRitual() )
            {
                StartRitual();
            }           
        }
    }
    #endregion


    #region 仪式相关
    private void StartRitual()
    {
        HellsCall.Instance.SetCanStartRitual(false);        //仪式开始后将布尔设置为false，防止玩家反复开始仪式

        //在一定时间内，定期在房间内生成敌人（生成位置随机）    需要做的：敌人只会攻击仪式台，即使受到了玩家攻击（敌人相关的逻辑放在敌人脚本里）
        GenerateEnemyThroughTime(m_RitualDuration, m_EnemySpawnInterval);
    }



    private void GenerateEnemyThroughTime(float duration, float spawnInterval)
    {
        StartCoroutine(m_DurationTimer.WaitForDuration() );     //开始仪式计时

        //开始持续生成敌人
        m_EnemySpawnCoroutine = StartCoroutine(EnemySpawnCoroutine(m_EnemySpawnInterval) );
    }

    private IEnumerator EnemySpawnCoroutine(float spawnInterval)        //敌人生成的协程
    {
        while (true)            //反复的生成敌人
        {
            GenerateEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void GenerateEnemy()
    {
        //在房间内的随机位置生成敌人
        EnvironmentManager.Instance.GenerateEnemy(HellsCall.Instance.RitualRoomDoorController, EnemyPrefab);
    }



    private void FinishRitual()   //仪式结束后的逻辑
    {
        //停止敌人生成的循环
        if (m_EnemySpawnCoroutine != null)
        {
            StopCoroutine(m_EnemySpawnCoroutine);
        }
    }
    #endregion


    #region 动画帧事件
    private void SetAnimatorBool()
    {
        Core.Animator.SetBool("Hit", false);    //放在受击动画的最后几帧

        m_IsHit = false;
    }

    #endregion
}