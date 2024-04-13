using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;




public class UIConst    //���ڴ洢���������
{
    public const string PlayerStatusBar = "PlayerStatusBar";    //���״̬������HealthBar�ű����ʼ��
    public const string TransitionStagePanel = "TransitionStagePanel";    //������׶�����
}




public class UIManager
{
    private static UIManager m_Instance;
    public static UIManager Instance    //����ģʽ��������Ϸֻ����һ�������ʵ����
    {
        get
        {
            if (m_Instance == null)     //��һ�μ��
            {
                m_Instance = new UIManager();
            }
            return m_Instance;
        }
    }


    private Transform m_UIRoot;
    public Transform UIRoot     //����UI�ĸ��ڵ㣨���ĸ����壩
    {
        get
        {
            if (m_UIRoot == null)
            {
                GameObject canvasObject = GameObject.Find("Canvas");

                if (canvasObject != null)
                {
                    m_UIRoot = canvasObject.transform;
                }
                else
                {
                    m_UIRoot = new GameObject("Canvas").transform;
                }               
            }
            return m_UIRoot;
        }
    }




    public Dictionary<string, BasePanel> PanelDict;      //����Ѵ򿪽�����ֵ䣨����洢�Ķ������ڴ򿪵Ľ��棩
    Dictionary<string, GameObject> m_PrefabDict;     //Ԥ�Ƽ������ֵ�








    //���캯��
    private UIManager()     
    {
        InitDicts();
    }


    //��ʼ���ֵ�
    private void InitDicts()    
    {
        PanelDict = new Dictionary<string, BasePanel>();
        m_PrefabDict = new Dictionary<string, GameObject>();
    }






    //�򿪽���
    public IEnumerator OpenPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            yield break;
        }

        GameObject panelPrefab;
        if (!m_PrefabDict.TryGetValue(name, out panelPrefab))
        {
            //�첽����Ԥ�Ƽ�
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            yield return new WaitUntil(() => handle.IsDone);

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load the panel prefab: " + name);
                yield break;
            }

            panelPrefab = handle.Result;
            m_PrefabDict[name] = panelPrefab;  //������ص�Ԥ�Ƽ�
        }


        GameObject panelObject = GameObject.Instantiate(panelPrefab, UIRoot, false);
        BasePanel panel = panelObject.GetComponent<BasePanel>();
        PanelDict.Add(name, panel);
        yield return panel;  //����UI�����ڽ�һ���Ĵ���(��ѡ)
    }



    //�رս���
    public bool ClosePanel(string name)
    {
        BasePanel panel = null;

        if (!PanelDict.TryGetValue (name, out panel))     //�������Ƿ��Ѵ򿪣�û�򿪵Ļ��򱨴�
        {
            Debug.LogError("This panel is not opened yet: " + name);
            return false;
        }


        if (panel.CanvasGroup != null)
        {
            panel.FadeOut(panel.CanvasGroup, 1f);       //������Ե����Ļ����ȵ���
        }
        else
        {
            panel.ClosePanel();
        }

        return true;
    }



    //��ǰ���ؽ��棨��ǰ��Ԥ�Ƽ������ֵ䣬��ֹ���٣�(���򿪽��溯��һģһ����ֻ���������ɲ��򿪽���Ĳ���)
    public IEnumerator InitPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            yield break;
        }

        GameObject panelPrefab;
        if (!m_PrefabDict.TryGetValue(name, out panelPrefab))
        {
            //�첽����Ԥ�Ƽ�
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            yield return new WaitUntil(() => handle.IsDone);

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to load the panel prefab: " + name);
                yield break;
            }

            panelPrefab = handle.Result;
            m_PrefabDict[name] = panelPrefab;  //������ص�Ԥ�Ƽ�
        }
    }



    public void ChangePanelLayer(BasePanel thisPanel)   //�ı�UI����Ⱦ�㼶
    {

    }
}