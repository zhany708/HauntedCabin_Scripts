using UnityEngine;



public class EnemyDeath : Death
{
    public DoorController DoorController { get; private set; }

    



    public override void Die()
    {
        base.Die();

        if (DoorController != null)
        {
            DoorController.IncrementEnemyCount();     //增加敌人计数器的计数
        }  

        else
        {
            Debug.LogError("Cannot find the DoorController component!");
        }
    }




    public void SetDoorController(DoorController door)
    {
        DoorController = door;
    }
}