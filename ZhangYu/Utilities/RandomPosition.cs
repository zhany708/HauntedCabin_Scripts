using System.Collections.Generic;
using UnityEngine;



namespace ZhangYu.Utilities     //张煜文件夹用于以后所有游戏都可能会用到的函数，如计时器，生成随机坐标等
{
    public class RandomPosition
    {
        Vector2 m_LeftDownPosition;     //用于随机生成巡逻坐标
        Vector2 m_RightTopPosition;


        public RandomPosition(Vector2 leftDwonPos, Vector2 rightTopPos)
        {
            m_LeftDownPosition = leftDwonPos;
            m_RightTopPosition = rightTopPos;
        }





        public Vector2 GenerateSingleRandomPos()
        {
            Vector2 newPos = new Vector2(Random.Range(m_LeftDownPosition.x, m_RightTopPosition.x), Random.Range(m_LeftDownPosition.y, m_RightTopPosition.y));

            return newPos;
        }

        public List<Vector2> GenerateMultiRandomPos(int num)
        {
            //Debug.Log("Num is " + num);

            if (num > 0)
            {
                List<Vector2> newPos = new List<Vector2>();

                for (int i = 0; i < num; i++)       //先将所有坐标生成出来
                {
                    newPos.Add(GenerateSingleRandomPos());
                }


                if (num > 1)    //当坐标数多于1个时检查是否有重复坐标
                {
                    bool isDone = false;

                    while (!isDone)
                    {
                        isDone = true;      //如果未有坐标重合则退出循环

                        for (int i = 0; i < num - 1; i++)   //从第一个坐标开始检查，持续到倒数第二个坐标（最后一个坐标无需检查）
                        {
                            for (int j = i + 1; j < num; j++)       //每次检查从当前坐标的下一个坐标开始，到最后一个坐标（当前坐标之前的已经检查过了）
                            {
                                if (IsOverlap(newPos[i], newPos[j]))
                                {
                                    newPos[i] = GenerateSingleRandomPos();    //如果重复了，则重新生成坐标，并进行新的循环

                                    isDone = false;
                                }
                            }
                        }
                    }
                }

                return newPos;
            }

            return null;
        }




        private bool IsOverlap(Vector2 firstPos, Vector2 secondPos)     //检查两个坐标是否几乎重合
        {
            if ( (Mathf.Abs(secondPos.x - firstPos.x) <= 1f) && (Mathf.Abs(secondPos.y - firstPos.y) <= 1f) )   //如果两个坐标的x和y的差值都小于1则视为重合
            {
                return true;
            }

            return false;
        }
    }
}