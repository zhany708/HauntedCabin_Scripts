using UnityEngine;



public class Event : MonoBehaviour
{
    public SO_EventData EventData;
    public int EventIndex = -1;          //事件的序列号，每个事件都不一样，具体可以查看云端里的“事件信息”文档


    protected DoorController doorController;





    #region Unity内部函数
    protected virtual void Awake()
    {
        if (EventData == null || EventIndex < 0)
        {
            Debug.LogError("Some components are not set in the: " + gameObject.name);
            return;
        }
    }
    #endregion


    #region 事件相关
    public virtual void StartEvent() { }

    protected virtual void FinishEvent()
    {
        EventManager.Instance.IncrementEventCount();     //增加触发过的事件计数

        if (doorController != null)
        {
            doorController.OpenDoors();
        }
        else
        {
            Debug.LogError("DoorController not set for " + gameObject.name);
            return;
        }

        EventManager.Instance.DeactivateEventObject();        //将事件物体放回池中
    }
    #endregion


    #region Setters
    public void SetDoor(DoorController thisdoorController)
    {
        if (thisdoorController == null)
        {
            Debug.LogError("Attempted to set a null DoorController on " + gameObject.name);
            return;
        }
        else
        {
            doorController = thisdoorController;
        }      
    }
    #endregion
}