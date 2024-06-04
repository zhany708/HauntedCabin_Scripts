public class PlayerStateMachine
{
    public PlayerState currentState {  get; private set; }  //创造一个公共Getter和一个私人Setter给这个变量

    public void Initialize(PlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();   
    }

    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
