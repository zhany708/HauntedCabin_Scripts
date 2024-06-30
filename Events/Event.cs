using UnityEngine;



public abstract class Event : MonoBehaviour
{
    public SO_EventData EventData;


    protected DoorController doorController;





    #region Unity内部函数
    protected virtual void Awake()
    {
        if (EventData == null)
        {
            Debug.LogError("EventData is not set on " + gameObject.name);
            return;
        }
    }
    #endregion


    #region 事件相关
    public abstract void StartEvent();

    protected void FinishEvent()
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