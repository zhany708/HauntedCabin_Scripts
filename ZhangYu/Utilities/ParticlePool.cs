using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour       //用于子弹，特效等的对象池
{
    public static ParticlePool Instance {  get; private set; }



    private Dictionary<string, Queue<GameObject>> m_ParticlePool = new Dictionary<string, Queue<GameObject>>();     //使用字典对不同的物体进行分开存储

    private GameObject m_Pool;      //所有生成物体的根父物体



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

            m_Pool = new GameObject("ParticlePool");

            //防止加载场景后跟父物体被删除
            DontDestroyOnLoad(m_Pool);
        }

    }


    //获取物体
    public GameObject GetObject(GameObject prefab)
    {
        //检查池中有没有物体，没有的话则新建一个并加进去
        if (!m_ParticlePool.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab);
            PushObject(newObject);
        }

        var obj = m_ParticlePool[prefab.name].Dequeue();
        obj.SetActive(true);

        return obj;

    }



    private GameObject CreateNewObject(GameObject prefab)
    {
        //先找父物体，随后再创建
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, childContainer.transform);

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

            if (!m_ParticlePool.ContainsKey(name))
            {
                //如果池中没有物体，则创建一个并加进去
                m_ParticlePool[name] = new Queue<GameObject>();
            }

            m_ParticlePool[name].Enqueue(obj);

            //创建完后取消激活
            obj.SetActive(false);
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }
     
        return false;
    }
}