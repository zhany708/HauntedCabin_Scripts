using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;



public class UIManager : ManagerTemplate<UIManager>
{
    //[SerializeField]    //ǿ�ȱ༭����ʾһ���ڲ�����
    public SO_UIKeys UIKeys;

    //����Ѵ򿪽�����ֵ䣨����洢�Ķ������ڴ򿪵Ľ��棩
    public Dictionary<string, BasePanel> PanelDict = new Dictionary<string, BasePanel>();      


    Transform m_UIRoot;     //���ڴ������е�UI��Ϊ�����ۣ�





    



    protected override void Awake()
    {
        base.Awake();

        //Ѱ�һ��������壬û�еĻ��ʹ���һ��
        SetupRootGameObject(ref m_UIRoot, "Canvas");

        //�����³���ʱ��ɾ��Canvas�����
        DontDestroyOnLoad(m_UIRoot);
    }


    private async void Start()
    {
        //����Ƿ��ڳ���0�����˵���
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            //��Ϸ��ʼʱ���ؿ�ʼ����
            //await OpenPanel(UIKeys.MainMenuPanel);
        }

        else
        {
            //�������˵�ʱ���򲥷�һ¥BGM
            await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true);
        }
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


        //�����Ϊ�������ص�ԭ���»��������屻ɾ�����������»�ȡ
        if (m_UIRoot == null)
        {
            //Ѱ�һ��������壬û�еĻ��ʹ���һ��
            SetupRootGameObject(ref m_UIRoot, "Canvas");
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


        if (!PanelDict.ContainsKey(name))
        {
            //������ӽ��������ڴ򿪽�����ֵ�
            PanelDict.Add(name, panel);
        }         
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

        panel.ClosePanel();
       
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


    public void ResetGame()
    {
        foreach (Transform child in m_UIRoot)    //�ڳ�����ɾ������Canvas�µ�UI
        {
            BasePanel childScript = child.GetComponent<BasePanel>();
            if (childScript == null)
            {
                Debug.LogError("Cannot get the BasePanel script from the: " + child.name);
                return;
            }

            childScript.ClosePanel();    
        }
    }
}