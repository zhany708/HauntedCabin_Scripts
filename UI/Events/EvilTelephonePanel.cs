using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using System.Collections;



public class EvilTelephonePanel : PanelWithButton
{
    public TextMeshProUGUI EventInfo;     //�¼������ı�
    public TextMeshProUGUI ResultText;     //ѡ�����ı�


    //�ĸ���ť
    public Button OptionA;
    public Button OptionB;
    public Button OptionC;
    public Button OptionD;




    //��ť���ı����
    TextMeshProUGUI m_OptionAText;      
    TextMeshProUGUI m_OptionBText;     
    TextMeshProUGUI m_OptionCText;      
    TextMeshProUGUI m_OptionDText;




    //���ڰ�ť������ö��
    private enum ButtonAction
    {
        OptionA,
        OptionB,
        OptionC,
        OptionD
    }





    protected override void Awake()
    {
        base.Awake();

        CheckComponents();      //����������

        //Ĭ�ϰ�ťΪ�����ĸ�ѡ���ť
        firstSelectedButton = OptionD.gameObject;
    }


    private void Start()
    {
        panelName = "Test";


        //����ť�ͺ���������
        OptionA.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionA));
        OptionB.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionB));
        OptionC.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionC));
        OptionD.onClick.AddListener(() => OnButtonClicked(ButtonAction.OptionD));


        //��ʹ�ô��ֻ�ǰ��ǰ�����ı��Ĳ�����ɫ����Ҫ���Ķ������ʱ�����ں����ٴε���.Replace�������ٴ����µ�string������
        string preparedText = EventInfo.text.Replace("answer", "<color=red>answer</color>");
        EventInfo.text = preparedText;

        StartTextAnimations();
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        SetButtons(false);      //����մ�ʱȡ���������а�ť

        //������ȫ��������ô˺���
        OnFadeOutFinished += ClosePanel;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //ClearOngoingCoroutine();
        OnFadeOutFinished -= ClosePanel;
    }






    private void OnButtonClicked(ButtonAction action)
    {
        switch (action)
        {
            case ButtonAction.OptionA:
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'Snacks, my favorite snacks!'\n\nSanity <#3D88FF>+1";

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionB:
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'The little pies can't escape my sight...'\n\nKnowledge <#3D88FF>+1";

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionC:
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'Baby, give me a kiss...'\n\nSanity <#3D88FF>-1";

                CommonLogicForOptions();
                break;

            case ButtonAction.OptionD:              
                //��ֵѡ��Ľ���ı�
                ResultText.text = $"???: 'Naughty kid must be punished!'\n\nStrength <#FF6B6B>-1";

                CommonLogicForOptions();
                break;

            default:
                Debug.Log("No Button is pressed.");
                break;
        }
    }




    private void CommonLogicForOptions()        //����ѡ���ͨ���߼�
    {
        SetButtons(false);      //ȡ���������а�ť

        //�ȿ�ʼ����ı��Ĵ���Ч��
        Coroutine resultTextCoroutine = StartCoroutine(TypeText(ResultText, ResultText.text, 0.05f, () =>       
        {
            //Debug.Log("First Coroutine done!.");

            Coroutine waitForInputCoroutine = StartCoroutine(WaitForPlayerInput(() =>     //�ȴ���Ұ��ո�������
            {
                //Debug.Log("Second Coroutine done!.");

                //��������
                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
            }));

            generatedCoroutines.Add(waitForInputCoroutine);    //��Э�̼ӽ��б�
        }));

        generatedCoroutines.Add(resultTextCoroutine);      //��Э�̼ӽ��б�
    }



    private IEnumerator WaitForPlayerInput(Action onInputReceived)      //�ȴ���Ұ��ո�����
    {
        bool inputReceived = false;     //��ʾ�Ƿ���ܵ���ҵ��źţ����ھ����Ƿ����ѭ��

        while (!inputReceived)
        {
            //�������Ƿ��¿ո����������
            if (playerInputHandler.IsSpacePressed || playerInputHandler.AttackInputs[(int)CombatInputs.primary])
            {
                inputReceived = true;
                onInputReceived?.Invoke();

                yield break;
            }

            yield return null;  //�ȴ�����һ֡Ϊֹ���Ӷ��ٴμ��
        }
    }




    private void SetButtons(bool isActive)
    {
        OptionA.gameObject.SetActive(isActive);
        OptionB.gameObject.SetActive(isActive);
        OptionC.gameObject.SetActive(isActive);
        OptionD.gameObject.SetActive(isActive);       
    }


    private void StartTextAnimations()
    {       
        if (EventInfo.text.Length > 0)      //�ȼ������ı��Ƿ�����
        {
            Coroutine eventInfoCoroutine = StartCoroutine(TypeText(EventInfo, EventInfo.text, 0.05f, () =>
            {
                SetButtons(true);       //�¼�������Ϻ󣬼������а�ť

                //ͬʱ��������ѡ�ť���ı�
                Coroutine OptionATextCoroutine = StartCoroutine(TypeText(m_OptionAText, m_OptionAText.text, 0.05f));
                Coroutine OptionBTextCoroutine = StartCoroutine(TypeText(m_OptionBText, m_OptionBText.text, 0.05f));
                Coroutine OptionCTextCoroutine = StartCoroutine(TypeText(m_OptionCText, m_OptionCText.text, 0.05f));
                Coroutine OptionDTextCoroutine = StartCoroutine(TypeText(m_OptionDText, m_OptionDText.text, 0.05f));

                generatedCoroutines.Add(OptionATextCoroutine);       //��Э�̼ӽ��б�
                generatedCoroutines.Add(OptionBTextCoroutine);
                generatedCoroutines.Add(OptionCTextCoroutine);
                generatedCoroutines.Add(OptionDTextCoroutine);
            }));

            generatedCoroutines.Add(eventInfoCoroutine);       //��Э�̼ӽ��б�
        }

        else
        {
            Debug.LogError("EventInfo text is empty.");
            return;
        }
    }





    private void CheckComponents()
    {
        //��鰴ť������¼������ı�����Ƿ����
        if (OptionA == null || OptionB == null || OptionC == null || OptionD == null)
        {
            Debug.LogError("Some buttons are not assigned in the EvilTelephonePanel.");
            return;
        }

        if (EventInfo == null || ResultText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the EvilTelephonePanel.");
            return;
        }

        


        //��ȡ���а�ť���ı����
        m_OptionAText = OptionA.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionBText = OptionB.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionCText = OptionC.GetComponentInChildren<TextMeshProUGUI>();
        m_OptionDText = OptionD.GetComponentInChildren<TextMeshProUGUI>();

        //��鰴ť���ı�����Ƿ����
        if (m_OptionAText == null || m_OptionBText == null || m_OptionCText == null || m_OptionDText == null)
        {
            Debug.LogError("Some option texts are not assigned on the buttons in the EvilTelephonePanel.");
            return;
        }
    }
}