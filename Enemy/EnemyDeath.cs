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
    }




    public void SetDoorController(DoorController door)
    {
        DoorController = door;
    }
}
