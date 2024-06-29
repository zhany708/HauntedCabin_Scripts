using System.Collections.Generic;
using UnityEngine;



public class EnemyPool : MonoBehaviour       //用于生成敌人的对象池，生成出来的所有敌人都放在子物体
{
    public static EnemyPool Instance { get; private set; }




    //储存所有敌人的字典
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

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }       
        }
    }



    //获取物体，第二个参数为敌人的生成坐标
    public GameObject GetObject(GameObject prefab, Vector2 spawnPos)
    {
        //检查池中有没有物体，没有的话则新建一个并加进去
        if (!m_EnemyPool.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab, spawnPos);

            if (!PushObject(newObject))
            {
                Debug.LogError("The parametr you use for the PushObject function is null!");
            }         
        }

        //从队列中获取物体
        var obj = m_EnemyPool[prefab.name].Dequeue();

        //在物体激活前赋予坐标给敌人的跟物体，这样才能正确的初始化生成巡逻点的脚本
        obj.transform.position = spawnPos;

        obj.SetActive(true);

        return obj;
    }
    


    private GameObject CreateNewObject(GameObject prefab, Vector2 spawnPos)
    {
        //先找用来存放每类敌人的父物体，随后再创建物体并放在对应的父物体下面
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
                //如果池中没有物体，则创建一个相应的队列
                m_EnemyPool[name] = new Queue<GameObject>();
            }

            m_EnemyPool[name].Enqueue(obj);     //将创建好的物体加进队列

            //Debug.Log("There are " + m_EnemyPool[name].Count + " enemys in the queue: " + name);

            //创建完后取消激活
            obj.SetActive(false);

            //随后设置父物体
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }

        return false;
    }





    //每当加载新场景时调用的函数
    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        //每当进入一楼场景时都调用以下逻辑
        if (scene.name == "FirstFloor")
        {
            
        }

        //进入其余场景时（目前只有主菜单）
        else
        {
            //重置游戏
            ResetGame()
        }
    }
    

    public void ResetGame()     //重置游戏（在加载其他场景前调用此函数）
    {
        //在场景中取消激活所有敌人
        foreach (Transform child in transform)    
        {
            foreach (Transform child2 in child)     //检索每一个子物体的子物体
            {
                //如果敌人处于激活状态（还没有死亡），则放回池中
                if (child2.CompareTag("Enemy") && child2.gameObject.activeSelf)
                {
                    //这里必须放回池中，不能单纯的取消激活。否则在敌人存活时重置游戏后，将不会重复使用之前生成过的敌人物体
                    PushObject(child2.gameObject);      //将敌人脚本的父物体放回池中，也将放回父物体的所有子物体
                }
            }
        }
    }

    public void KillAllEnemy()     //结束游戏（在玩家胜利时调用此函数）
    {
        //让所有对象池中的敌人立刻进入死亡状态
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

    public void KillAllEnemy_DefenseWar()     //结束游戏（在玩家胜利时调用此函数）
    {
        //让所有对象池中的保卫战中的敌人立刻进入死亡状态
        foreach (Transform child in transform)
        {
            foreach (Transform child2 in child)     //检索每一个子物体的子物体
            {
                //检查物体是否有敌人标签，且处于激活状态
                if (child2.CompareTag("Enemy") && child2.gameObject.activeSelf)
                {
                    Enemy_DefenseWar enemyScript = child2.GetComponentInChildren<Enemy_DefenseWar>();     //从子物体那获取敌人脚本

                    if (enemyScript == null)
                    {
                        Debug.LogError("Cannot get the Enemy script from the children Objects.");
                        return;
                    }

                    enemyScript.StateMachine.ChangeState(enemyScript.DeathState);   //强行让敌人进入死亡状态                 
                }
            }
        }


        /*
        //再次检查，确保是否有敌人出现异常（比如卡死在原地没有消失等）
        foreach (Transform child in transform)
        {
            foreach (Transform child2 in child)     //检索每一个子物体的子物体
            {
                //检查物体是否有敌人标签，且处于激活状态
                if (child2.CompareTag("Enemy") && child2.gameObject.activeSelf)
                {
                    //如果此时仍然有敌人处于激活状态，则强行放回池中
                    PushObject(child2.gameObject);      //将敌人脚本的父物体放回池中，也将放回父物体的所有子物体
                }
            }
        }
        */
    }
}