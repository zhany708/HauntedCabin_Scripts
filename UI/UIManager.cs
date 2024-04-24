using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;



public class UIManager : ManagerTemplate<UIManager>
{
    [SerializeField]
    public SO_UIKeys UIKeys;


    public Dictionary<string, BasePanel> PanelDict = new Dictionary<string, BasePanel>();      //����Ѵ򿪽�����ֵ䣨����洢�Ķ������ڴ򿪵Ľ��棩


    Transform m_UIRoot;     //���ڴ������е�UI��Ϊ�����ۣ�









    protected override void Awake()
    {
        base.Awake();

        //Ѱ�һ������壬û�еĻ��ʹ���һ��
        GameObject canvasObject = GameObject.Find("Canvas");
        if (canvasObject == null)
        {
            canvasObject = new GameObject("Canvas");
        }

        m_UIRoot = canvasObject.transform;

        //�����³���ʱ��ɾ��Canvas�����
        DontDestroyOnLoad(canvasObject);
    }


    private async void Start()
    {
        //��Ϸ��ʼʱ���ؿ�ʼ����
        await OpenPanel(UIKeys.MainMenuPanel);
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
        GameObject panelPrefab = await LoadPrefabAsync(name);
        if (panelPrefab == null)
        {
            Debug.LogError("Failed to load panel prefab: " + name);
            return;
        }
        

        //�첽���غ��������岢��ȡ�������ϵ����
        GameObject panelObject = GameObject.Instantiate(panelPrefab, m_UIRoot, false);
        BasePanel panel = panelObject.GetComponent<BasePanel>();
        if (panel != null)
        {
            //��ȡ����󣬴򿪽���
            panel.OpenPanel(name);
        }

        else
        {
            Debug.LogError("No BasePanel component found on prefab: " + name);
            return;
        }

        //������ӽ��������ڴ򿪽�����ֵ�
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

        await LoadPrefabAsync(name);
    }




    public void ChangePanelLayer(BasePanel thisPanel)   //�ı�UI����Ⱦ�㼶
    {

    }
}