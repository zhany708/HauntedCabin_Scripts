using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class ParticlePool : MonoBehaviour       //�����ӵ�����Ч�ȵĶ���ء����ɳ������������嶼����������
{
    public static ParticlePool Instance {  get; private set; }


    //ʹ���ֵ�Բ�ͬ��������зֿ��洢
    private Dictionary<string, Queue<GameObject>> m_ParticlePoolDict = new Dictionary<string, Queue<GameObject>>();     







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
        }
    }


    //��ȡ����
    public GameObject GetObject(GameObject prefab)
    {
        //��������û�����壬û�еĻ����½�һ�����ӽ�ȥ
        if (!m_ParticlePoolDict.TryGetValue(prefab.name, out var queue) || queue.Count == 0)
        {
            var newObject = CreateNewObject(prefab);
            PushObject(newObject);
        }

        var obj = m_ParticlePoolDict[prefab.name].Dequeue();
        obj.SetActive(true);

        return obj;
    }



    private GameObject CreateNewObject(GameObject prefab)
    {
        //�����������ÿ������ĸ����壬����ٴ���
        GameObject childContainer = FindOrCreateChildContainer(prefab.name);
        GameObject obj = Instantiate(prefab, childContainer.transform);

        return obj;
    }

    //����Ѱ�һ򴴽�������ĸ����壨Ϊ���������ۣ�
    private GameObject FindOrCreateChildContainer(string name)
    {
        //�ȳ����ڵ�ǰ�������������Ѱ�Ҳ����е����ֵ�����
        Transform childTransform = transform.Find(name);

        if (childTransform == null)
        {
            GameObject child = new GameObject(name);

            //��ÿ�����͵�����ĸ���������Ϊ��ǰ�����������
            child.transform.SetParent(transform);

            return child;
        }

        return childTransform.gameObject;
    }





    //��������
    public bool PushObject(GameObject obj)
    {
        if (obj != null)
        {
            //��ȡ��������
            string name = obj.name;

            if (name.EndsWith("(Clone)"))
            {
                //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
                name = name.Substring(0, name.Length - 7);
            }



            if (!m_ParticlePoolDict.ContainsKey(name))
            {
                //�������û�����壬�򴴽�һ�����ӽ�ȥ
                m_ParticlePoolDict[name] = new Queue<GameObject>();
            }

            m_ParticlePoolDict[name].Enqueue(obj);

            //�������ȡ������
            obj.SetActive(false);

            //������ø�����
            obj.transform.SetParent(FindOrCreateChildContainer(name).transform);

            return true;
        }
     
        return false;
    }
}