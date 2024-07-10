using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class MiniMapPanel : BasePanel
{
    public static MiniMapPanel Instance { get; private set; }









    protected override void Awake()
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

    private void Start()
    {
        //设置当前界面的名字
        if (panelName == null)
        {
            panelName = UIManager.Instance.UIKeys.MiniMapPanel;
        }


        //检查该界面是否是唯一保留的那个
        if (Instance == this)
        {
            if (!UIManager.Instance.PanelDict.ContainsKey(panelName))
            {
                //Debug.Log(panelName + " added into the PanelDict!");

                //将界面加进字典，表示界面已经打开
                UIManager.Instance.PanelDict.Add(panelName, this);
            }


            if (!UIManager.Instance.ImportantPanelList.Contains(this))
            {
                //将该界面加进重要界面列表，以在重置游戏时不被删除
                UIManager.Instance.ImportantPanelList.Add(this);
            }
        }
    }


    protected override void OnDisable()
    {
        //先检查界面名字是否为空（为空的话则代表当前界面是重复的，因为是在Start函数中赋值名字）
        if (panelName != null)
        {
            //检查该界面是否是唯一保留的那个
            if (Instance == this)
            {
                if (UIManager.Instance.PanelDict.ContainsKey(panelName))
                {
                    //从字典中移除，表示界面没打开
                    UIManager.Instance.PanelDict.Remove(panelName);
                }


                if (UIManager.Instance.ImportantPanelList.Contains(this))
                {
                    //从重要界面列表中移除当前界面
                    UIManager.Instance.ImportantPanelList.Remove(this);
                }
            }
        }
    }
}
