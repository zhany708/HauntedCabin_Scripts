using System;
using System.Collections;
using UnityEngine;




public class Delay : MonoBehaviour      //用于处理延迟相关的脚本
{
    public static Delay Instance { get; private set; }






    #region Unity内部函数
    private void Awake()
    {
        //单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;

            //只有在没有父物体时才运行防删函数，否则会出现提醒
            if (gameObject.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
    #endregion


    #region 主要函数
    public IEnumerator DelaySomeTime(float delay, Action onTimerDone = null)      //用于延迟一段时间后执行一些逻辑
    {
        yield return new WaitForSeconds(delay);

        onTimerDone?.Invoke();
    }



    //等待玩家按空格或鼠标
    public IEnumerator WaitForPlayerInput(Action onInputReceived = null)      
    {
        bool inputReceived = false;     //表示是否接受到玩家的信号，用于决定是否结束循环

        while (!inputReceived)
        {
            //检查玩家是否按下空格或点击鼠标左键
            if (PlayerInputHandler.Instance.IsSpacePressed || PlayerInputHandler.Instance.AttackInputs[(int)CombatInputs.primary])
            {
                inputReceived = true;

                //等待0.15秒再调用回调，否则如果此函数结束后的下一个函数也需要按空格时，可能会导致按一次空格响应多个函数
                yield return new WaitForSeconds(0.15f);

                onInputReceived?.Invoke();

                yield break;
            }

            yield return null;  //等待到下一帧为止，从而再次检查
        }
    }
    #endregion
}