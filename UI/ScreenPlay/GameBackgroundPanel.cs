using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameBackgroundPanel : BasePanel
{
    public TextMeshProUGUI FirstPartText;       //第一段文本
    public TextMeshProUGUI SecondPartText;      //第二段文本
    public TextMeshProUGUI LastPartText;        //最后一段文本
    public TextMeshProUGUI TipText;             //提示文本




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
        //界面完全淡出后调用以下函数
        OnFadeOutFinished += ClosePanel;
        OnFadeOutFinished += PlayGame;

        OnFadeInFinished += StartTextAnimations;    //界面完全淡入后调用此函数
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnFadeOutFinished -= ClosePanel;
        OnFadeOutFinished -= PlayGame;

        OnFadeInFinished -= StartTextAnimations;
    }




    private async void PlayGame()   //开始游戏
    {
        //载入一楼大厅场景
        SceneManager.LoadScene("FirstFloor");

        //播放一楼BGM
        await SoundManager.Instance.PlayBGMAsync(SoundManager.Instance.AudioClipKeys.StopForAMoment, true, SoundManager.Instance.MusicVolume);
    }



    private void StartTextAnimations()
    {
        FirstPartText.gameObject.SetActive(true);       //激活第一段文本

        //先开始第一段文本的打字效果
        Coroutine firstPartTextCoroutine = StartCoroutine(TypeText(FirstPartText, FirstPartText.text, () =>
        {
            TipText.gameObject.SetActive(true);     //激活提示文本，提醒玩家按空格或点击以继续游戏

            //等待玩家按空格或点击鼠标
            Coroutine firstWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
            {
                SecondPartText.gameObject.SetActive(true);      //激活第二段文本
                TipText.gameObject.SetActive(false);     //取消激活提示文本


                //然后开始第二段文本的打字效果
                Coroutine secondPartTextCoroutine = StartCoroutine(TypeText(SecondPartText, SecondPartText.text, () =>
                {
                    //等待1秒
                    Coroutine delayCoroutine = StartCoroutine(Delay.Instance.DelaySomeTime(1f, () =>
                    {
                        LastPartText.gameObject.SetActive(true);      //激活最后一段文本


                        //然后开始最后一段文本的打字效果
                        Coroutine lastPartCoroutine = StartCoroutine(TypeText(LastPartText, LastPartText.text, () =>
                        {
                            TipText.gameObject.SetActive(true);     //激活提示文本

                            //等待玩家按空格或点击鼠标
                            Coroutine lastWaitForInputCoroutine = StartCoroutine(Delay.Instance.WaitForPlayerInput(() =>
                            {
                                //Debug.Log("Last Coroutine done!.");

                                //淡出界面
                                Fade(CanvasGroup, FadeOutAlpha, FadeDuration, false);
                            }));

                            generatedCoroutines.Add(lastWaitForInputCoroutine);    //将协程加进列表
                        }));

                        generatedCoroutines.Add(lastPartCoroutine);    //将协程加进列表
                    }));

                    generatedCoroutines.Add(delayCoroutine);    //将协程加进列表
                }));

                generatedCoroutines.Add(secondPartTextCoroutine);   //将协程加进列表

            }));

            generatedCoroutines.Add(firstWaitForInputCoroutine);    //将协程加进列表
        }));

        generatedCoroutines.Add(firstPartTextCoroutine);      //将协程加进列表
    }
}