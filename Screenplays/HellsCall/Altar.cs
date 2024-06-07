using System.Collections;
using UnityEngine;
using ZhangYu.Utilities;



public class Altar : MonoBehaviour     //放在仪式台上的脚本
{
    public GameObject EnemyPrefab;      //敌人的预制件


    Timer m_DurationTimer;      //用于计时仪式时长的计时器


    float m_RitualDuration = 9f;        //仪式时间
    float m_EnemySpawnInterval = 3f;    //敌人生成的冷却

    Coroutine m_EnemySpawnCoroutine;    //敌人生成的协程





    private void Awake()
    {
        //初始化计数器
        m_DurationTimer = new Timer(m_RitualDuration);
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
}