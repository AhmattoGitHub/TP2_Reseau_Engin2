
public abstract class CharacterState : GM_IState
{

    protected RunnerSM m_stateMachine;
    public void OnStart(RunnerSM controller)
    {
        m_stateMachine = controller;
    }

    public void OnStart()
    {
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnFixedUpdate()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual bool CanEnter(GM_IState currentState)
    {
        return true;
    }
    public virtual bool CanExit()
    {
        return true;
    }

}
