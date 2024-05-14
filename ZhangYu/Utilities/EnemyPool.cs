using System.Collections.Generic;
using UnityEngine;



public class EnemyPool : MonoBehaviour       //用于生成敌人的对象池，生成出来的所有敌人都放在子物体
{
    public static EnemyPool Instance { get; private set; }


    Dictionary<string, Queue<GameObject>> m_EnemyPool = new Dictionary<string, Queue<GameObject>>();








    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }



    //获取物体，第二个参数为敌人的生成坐标
    public GameObject GetObject(GameObject prefab, Vector2 spawnPos)
    {
        //检查池中有没有物体，没有的话则新建一个并加进去
        if (!m_EnemyPool.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab, spawnPos);
            PushObject(newObject);
        }

        var obj = m_EnemyPool[prefab.name].Dequeue();

        //在物体激活前赋予坐标给敌人的跟物体，这样才能正确的初始化生成巡逻点的脚本
        obj.transform.position = spawnPos;

        obj.SetActive(true);

        return obj;
    }



    private GameObject CreateNewObject(GameObject prefab, Vector2 spawnPos)
    {
        //先找用来存放每类敌人的父物体，随后再创建
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, childContainer.transform);

        return obj;
    }

    //用于寻找或创建子物体的父物体（为了整洁美观）
    private GameObject FindOrCreateChildContainer(string name)
    {
        //先尝试在当前物体的子物体中寻找参数中的名字的物体
        Transform childTransform = transform.Find(name);

        if (childTransform == null)
        {
            GameObject child = new GameObject(name);

            //将每种类型的敌人的父物体设置为当前物体的子物体
            child.transform.SetParent(transform);

            return child;
        }

        return childTransform.gameObject;
    }





    //储存物体
    public bool PushObject(GameObject obj)
    {
        if (obj != null)
        {
            //去掉物体名字的后缀
            string name = obj.name.Replace("(Clone)", "").Trim();

            if (!m_EnemyPool.ContainsKey(name))
            {
                //如果池中没有物体，则创建一个并加进去
                m_EnemyPool[name] = new Queue<GameObject>();
            }

            m_EnemyPool[name].Enqueue(obj);

            //创建完后取消激活
            obj.SetActive(false);

            //随后设置父物体
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }

        return false;
    }



    public void ResetGame()     //重置游戏（在加载其他场景前调用此函数）
    {
        //在场景中取消激活所有敌人
        foreach (Transform child in transform)    
        {
            foreach (Transform child2 in child)     //检索每一个子物体的子物体
            {
                if (child2.CompareTag("Enemy"))
                {
                    //这里必须放回池中，不能单纯的取消激活。否则在敌人存活时重置游戏后，将不会重复使用之前生成过的敌人物体
                    PushObject(child2.gameObject);      //将敌人脚本的父物体放回池中，也将放回父物体的所有子物体
                }
            }
        }
    }

    public void EndGame()     //结束游戏（在玩家胜利时调用此函数）
    {
        //在场景中让所有激活的敌人进入死亡状态
        foreach (Transform child in transform)
        {
            foreach (Transform child2 in child)     //检索每一个子物体的子物体
            {
                //检查物体是否有敌人标签，且处于激活状态
                if (child2.CompareTag("Enemy") && child2.gameObject.activeSelf)
                {
                    Enemy enemyScript = child2.GetComponentInChildren<Enemy>();     //从子物体那获取敌人脚本

                    if (enemyScript == null)
                    {
                        Debug.LogError("Cannot get the Enemy script from the children Objects.");
                        return;
                    }

                    enemyScript.StateMachine.ChangeState(enemyScript.DeathState);   //强行让敌人进入死亡状态                 
                }
            }
        }
    }
}