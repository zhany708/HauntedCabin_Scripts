using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameBackgroundPanel : BasePanel
{
    public TextMeshProUGUI FirstPartText;       //��һ���ı�
    public TextMeshProUGUI SecondPartText;      //�ڶ����ı�
    public TextMeshProUGUI LastPartText;        //���һ���ı�
    public TextMeshProUGUI TipText;             //��ʾ�ı�




    protected override void Awake()
    {
        if (FirstPartText == null || SecondPartText == null || LastPartText == null || TipText == null)
        {
            Debug.LogError("Some TMP components are not assigned in the GameBackgroundPanel.");
            return;
        }
    }


    private void OnEnable()
    {
        //������ȫ������������º���
        OnFadeOutFinished += ClosePanel;
        OnFadeOutFinished += PlayGame;

        OnFadeInFinished += StartTextAnimations;    //������ȫ�������ô˺���
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= PlayGame;

        OnFadeInFinished -= StartTextAnimations;
    }




    private async void PlayGame()   //��ʼ��Ϸ
    {
        //����һ¥��������
        SceneManager.LoadScene("FirstFloor");

        //����һ¥BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);
    }



    private void StartTextAnimations()
    {
        FirstPartText.gameObject.SetActive(true);       //�����һ���ı�

        //�ȿ�ʼ��һ���ı��Ĵ���Ч��
        Coroutine firstPartTextCoroutine = StartCoroutine(TypeText(FirstPartText, FirstPartText.text, () =>
        {
            TipText.gameObject.SetActive(true);     //������ʾ�ı���������Ұ��ո�����Լ�����Ϸ

            //�ȴ���Ұ��ո�������
            Coroutine firstWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
            {
                SecondPartText.gameObject.SetActive(true);      //����ڶ����ı�
                TipText.gameObject.SetActive(false);     //ȡ��������ʾ�ı�


                //Ȼ��ʼ�ڶ����ı��Ĵ���Ч��
                Coroutine secondPartTextCoroutine = StartCoroutine(TypeText(SecondPartText, SecondPartText.text, () =>
                {
                    //�ȴ�1��
                    Coroutine delayCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(1f, () =>
                    {
                        LastPartText.gameObject.SetActive(true);      //�������һ���ı�


                        //Ȼ��ʼ���һ���ı��Ĵ���Ч��
                        Coroutine lastPartCoroutine = StartCoroutine(TypeText(LastPartText, LastPartText.text, () =>
                        {
                            TipText.gameObject.SetActive(true);     //������ʾ�ı�

                            //�ȴ���Ұ��ո�������
                            Coroutine lastWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
                            {
                                //Debug.Log("Last Coroutine done!.");

                                //��������
                                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                            }));

                            generatedCoroutines.Add(lastWaitForInputCoroutine);    //��Э�̼ӽ��б�
                        }));

                        generatedCoroutines.Add(lastPartCoroutine);    //��Э�̼ӽ��б�
                    }));

                    generatedCoroutines.Add(delayCoroutine);    //��Э�̼ӽ��б�
                }));

                generatedCoroutines.Add(secondPartTextCoroutine);   //��Э�̼ӽ��б�

            }));

            generatedCoroutines.Add(firstWaitForInputCoroutine);    //��Э�̼ӽ��б�
        }));

        generatedCoroutines.Add(firstPartTextCoroutine);      //��Э�̼ӽ��б�
    }
}