using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;



public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }



    public Dictionary<string, BasePanel> PanelDict = new Dictionary<string, BasePanel>();      //����Ѵ򿪽�����ֵ䣨����洢�Ķ������ڴ򿪵Ľ��棩
    Dictionary<string, GameObject> m_PrefabDict = new Dictionary<string, GameObject>();     //Ԥ�Ƽ������ֵ�


    Transform m_UIRoot;     //���ڴ������е�UI��Ϊ�����ۣ�









    private void Awake()
    {
        //����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }

        //Ѱ�һ������壬û�еĻ��ʹ���һ��
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
        }

        m_UIRoot = canvasObject.transform;
    }






    //�򿪽���
    public async Task OpenPanel(string name)
    {
        //��������Ѿ��򿪣��򱨴�
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            return;
        }

        //�첽���أ�������Ƿ���سɹ�
        GameObject panelPrefab = await LoadPanelAsync(name);
        if (panelPrefab == null)
        {
            Debug.LogError("Failed to load panel prefab: " + name);
            return;
        }
        

        //�첽���غ��������岢��ȡ�������ϵ����
        GameObject panelObject = GameObject.Instantiate(panelPrefab, m_UIRoot, false);
        BasePanel panel = panelObject.GetComponent<BasePanel>();
        if (panel == null)
        {
            Debug.LogError("No BasePanel component found on prefab: " + name);
            return;
        }

        PanelDict.Add(name, panel);
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



    //��ǰ���ؽ��棨��ǰ��Ԥ�Ƽ������ֵ䣬��ֹ���٣�(���򿪽��溯������һģһ����ֻ���������ɲ��򿪽���Ĳ���)
    public async Task InitPanel(string name)
    {
        if (PanelDict.ContainsKey(name))
        {
            Debug.LogError("This panel is already opened: " + name);
            return;
        }

        await LoadPanelAsync(name);
    }

    //�첽����
    private async Task<GameObject> LoadPanelAsync(string name)
    {
        //����ֵ����Ƿ��н��棬����еĻ�ֱ�ӷ���
        if (!m_PrefabDict.TryGetValue(name, out GameObject panelPrefab))
        {
            //�첽���ؽ���
            var handle = Addressables.LoadAssetAsync<GameObject>(name);
            await handle.Task;

            //����첽�����Ƿ�ɹ�
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                panelPrefab = handle.Result;

                //������Ԥ�Ƽ�����ֵ�
                m_PrefabDict[name] = panelPrefab;
            }

            else
            {
                Debug.LogError($"Failed to load Panel prefab: {name}");
                return null;
            }
        }

        return panelPrefab;
    }





    public void ChangePanelLayer(BasePanel thisPanel)   //�ı�UI����Ⱦ�㼶
    {

    }


    //��Addressables���ͷ�UI��ֻ�����������ͷ��ڴ�
    public void ReleaseUI(string key)
    {
        if (key.EndsWith("(Clone)"))
        {
            //����Ƿ��С���¡����׺������еĻ���ȥ��׺����Clone���պ���7���ַ�
            key = key.Substring(0, key.Length - 7);
        }


        if (m_PrefabDict.TryGetValue(key, out GameObject panelPrefab))
        {
            Addressables.Release(panelPrefab);

            //��Ԥ�Ƽ������ֵ����Ƴ�UI����
            m_PrefabDict.Remove(key);

            Debug.Log("UI released and removed from dictionary: " + key);
        }

        else
        {
            Debug.LogError("This UI is not loaded yet, cannot release: " + key);
        }
    }
}