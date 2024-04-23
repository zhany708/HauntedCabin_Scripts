using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour       //�����ӵ�����Ч�ȵĶ����
{
    public static ParticlePool Instance {  get; private set; }



    private Dictionary<string, Queue<GameObject>> m_ParticlePool = new Dictionary<string, Queue<GameObject>>();     //ʹ���ֵ�Բ�ͬ��������зֿ��洢

    private GameObject m_Pool;      //������������ĸ�������



    private void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        else
        {
            Instance = this;

            m_Pool = new GameObject("ParticlePool");
        }

    }


    //��ȡ����
    public GameObject GetObject(GameObject prefab)
    {
        //��������û�����壬û�еĻ����½�һ�����ӽ�ȥ
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
        //���Ҹ����壬����ٴ���
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, childContainer.transform);

        return obj;
    }

    //����Ѱ�һ򴴽�������ĸ����壨Ϊ���������ۣ�
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





    //��������
    public bool PushObject(GameObject obj)
    {
        if (obj != null)
        {
            //ȥ���������ֵĺ�׺
            string name = obj.name.Replace("(Clone)", "").Trim();

            if (!m_ParticlePool.ContainsKey(name))
            {
                //�������û�����壬�򴴽�һ�����ӽ�ȥ
                m_ParticlePool[name] = new Queue<GameObject>();
            }

            m_ParticlePool[name].Enqueue(obj);

            //�������ȡ������
            obj.SetActive(false);
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }
     
        return false;
    }
}