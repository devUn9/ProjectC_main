using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected Rigidbody2D rb;

    private string animBoolName;

    protected Vector2 InputVector;
    protected Vector2 lastDirection;

    protected bool triggerCalled;

    public PlayerState(Player _player ,PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        InputVector.x = Input.GetAxisRaw("Horizontal");
        InputVector.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("АјАн");
            stateMachine.ChangeState(player.attackState);
        }
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }
    public Vector2 GetLastDirection()
    {
        return lastDirection;
    }

    public void SetAnimDirection(Vector2 _Velocity)
    {
        player.anim.SetFloat("VelocityX", _Velocity.x);
        player.anim.SetFloat("VelocityY", _Velocity.y);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }
}
