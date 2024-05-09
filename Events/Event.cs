using UnityEngine;



public abstract class Event : MonoBehaviour
{
    public SO_EventData EventData;


    protected DoorController doorController;






    protected virtual void Awake()
    {
        if (EventData == null)
        {
            Debug.LogError("EventData is not set on " + gameObject.name);
            return;
        }
    }



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
    }



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