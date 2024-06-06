using System;
using System.Collections;
using UnityEngine;




namespace ZhangYu.Utilities     //张煜文件夹用于以后所有游戏都可能会用到的函数，如计时器等
{
    public class Timer
    {
        public event Action OnTimerDone;    //接收对象有Weapon, Enemy, Altar



        float m_StartTime;      //开始时间
        float m_Duration;       //计时时长
        float m_TargetTime;     //结束时间

        bool m_IsActive;                //表示计时器是否激活（正在计时）
        bool m_IsTimerDone = false;     //表示时间是否已经到了（用于协程的计时）




        public Timer(float duration)
        {
            m_Duration = duration;
        }



        public void StartTimer()    //开始计时器
        {
            m_StartTime = Time.time;
            m_TargetTime = m_StartTime + m_Duration;
            m_IsActive = true;
            //m_IsTimerDone = false;
        }

        public void StopTimer()     //暂停计时器
        {
            m_IsActive = false;
        }


        
        public void Tick()      //使用系统自带的时间计时，中途可以暂停
        {
            if (!m_IsActive) return;


            if (Time.time > m_TargetTime)
            {
                //Debug.Log("Time up!");

                OnTimerDone?.Invoke();      //触发计时结束事件
                //m_IsTimerDone = true;
                StopTimer();    //到达目标时间后停止计时
            }
        }


        public IEnumerator WaitForDuration()        //使用协程计时，中途无法暂停
        {
            m_IsTimerDone = false;

            yield return new WaitForSeconds(m_Duration);        //等待一段时间

            //Debug.Log("Time up!");
            OnTimerDone?.Invoke();          //触发计时结束事件
            m_IsTimerDone = true;           //将布尔设置为false表示计时结束
        }






        #region Getters
        public bool GetIsTimerDone()
        {
            return m_IsTimerDone;
        }
        #endregion
    }
}