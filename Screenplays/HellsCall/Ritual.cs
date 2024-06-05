using UnityEngine;



public class Ritual : MonoBehaviour
{
    float m_RitualDuration = 0f;        //仪式时间
    float m_EnemySpawnInterval = 0f;    //敌人生成的冷却

    DoorController m_DoorController;




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //需要做的：在一定时间内，定期在房间内生成敌人（生成位置随机）。敌人只会攻击仪式台，即使受到了玩家攻击（敌人相关的逻辑放在敌人脚本里）

        }
    }




    private void GenerateEnemyThroughTime(float duration, float spawnInterval)
    {

    }
}