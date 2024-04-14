using System.Collections.Generic;
using UnityEngine;

public class ParticlePool       //用于子弹，特效等的对象池
{
    public static ParticlePool Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new ParticlePool();
            }
            return m_Instance;
        }
    }

    private static ParticlePool m_Instance;

    private Dictionary<string, Queue<GameObject>> m_ParticlePool = new Dictionary<string, Queue<GameObject>>();     //使用字典对不同的物体进行分开存储

    private GameObject m_Pool;      //所有生成物体的根父物体



    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;

        if (!m_ParticlePool.ContainsKey(prefab.name) || m_ParticlePool[prefab.name].Count == 0)       //检查池中是否有物体，或物体数量是否为0
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);

            if (m_Pool == null)
            {
                m_Pool = new GameObject("ParticlePool");   //生成储存所有池对象的总父物体
            }

            GameObject m_Child = GameObject.Find(prefab.name);
            if (!m_Child)
            {
                m_Child = new GameObject(prefab.name);     //生成每个池对象的父物体
                m_Child.transform.SetParent(m_Pool.transform);      //将每个父物体设置为总父物体的子物体
            }

            _object.transform.SetParent(m_Child.transform);     //将每个物体设置为其父物体的子物体
        }

        _object = m_ParticlePool[prefab.name].Dequeue();       //按照预制体名获取对象池中的预制体
        _object.SetActive(true);
        return _object;
    }

    public bool PushObject(GameObject prefab)
    {
        if (prefab != null)
        {
            string _name = prefab.name.Replace("(Clone)", string.Empty);    //将克隆后缀替换成空

            if (!m_ParticlePool.ContainsKey(_name))
            {
                m_ParticlePool.Add(_name, new Queue<GameObject>());    //查找物体名是否存在于对象池，若不存在则新生成一个
            }
            m_ParticlePool[_name].Enqueue(prefab);     //生成后将物体放入对象池

            prefab.SetActive(false);    //放入后取消激活

            return true;
        }     
        
        return false;
    }
}