using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour       //用于生成敌人的对象池
{
    public static EnemyPool Instance { get; private set; }


    Dictionary<string, Queue<GameObject>> m_EnemyPool = new Dictionary<string, Queue<GameObject>>();

    GameObject m_Pool;






    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;

            m_Pool = new GameObject("EnemyPool");
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
        //先找父物体，随后再创建
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity, childContainer.transform);

        return obj;
    }

    //用于寻找或创建子物体的父物体（为了整洁美观）
    private GameObject FindOrCreateChildContainer(string name)
    {
        Transform childTransform = m_Pool.transform.Find(name);

        if (childTransform == null)
        {
            GameObject child = new GameObject(name);
            child.transform.SetParent(m_Pool.transform);

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
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }

        return false;
    }
}