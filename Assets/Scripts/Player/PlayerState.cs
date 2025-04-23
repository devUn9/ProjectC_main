using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected Rigidbody2D rb;

    private string animBoolName;

    protected Vector2 InputVector;
    protected Vector2 lastDirection;

    protected bool triggerCalled;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update()
    {
        player.attackStateTimer -= Time.deltaTime;

        InputVector.x = Input.GetAxisRaw("Horizontal");
        InputVector.y = Input.GetAxisRaw("Vertical");

        if(animBoolName != "Idle")
        {
            if (player.attackStateTimer > 0)
            {
                PlayerToMousePosDir();
            }
            else
            {
                SetAnimDirection(InputVector);
            }
        }
        

        if (InputVector.x != 0 || InputVector.y != 0)
        {
            lastDirection = InputVector.normalized;
            stateMachine.ChangeState(player.moveState);
        }

        // 제자리 공격 애니매이션 종료 후 AnimationTrigger 발생 전까지 공격 입력불가능하게 조정
        // AnimationTrigger 발생 전 입력 시 state에서 에러발생 attackStateTimer변수는 player에서 관리
        if (Input.GetKeyDown(KeyCode.Mouse0) && player.attackStateTimer < 0.35)
        {
            if (animBoolName == "Move")
                stateMachine.ChangeState(player.pistolMove);
            else
                stateMachine.ChangeState(player.attackState);
        }
    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

    public void PlayerToMousePosDir()
    {
        Vector2 MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        Vector3 Pos = new Vector3(MousePos.x, MousePos.y, 0);
        Vector3 dir = Pos - player.transform.position;

        Vector3 dirNo = new Vector3(dir.x, dir.y, 0).normalized;
        //Debug.Log(dir.x + "," + dir.y);

        SetAnimDirection(dirNo);
    }

    public void SetAnimDirection(Vector2 _Velocity)
    {
        player.anim.SetFloat("VelocityX", _Velocity.x);
        player.anim.SetFloat("VelocityY", _Velocity.y);
    }
}
