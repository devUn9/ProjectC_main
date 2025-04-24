using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected Rigidbody2D rb;

    private string animBoolName;

    protected Vector2 stateInputVec;
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

        stateInputVec.x = Input.GetAxisRaw("Horizontal");
        stateInputVec.y = Input.GetAxisRaw("Vertical");

        if(animBoolName != "Idle")
        {
            if (player.attackStateTimer > 0)
            {
                PlayerToMousePosDir();
            }
            else
            {
                SetAnimDirection(stateInputVec);
            }
        }
        

        if (stateInputVec.x != 0 || stateInputVec.y != 0)
        {
            lastDirection = stateInputVec.normalized;
            stateMachine.ChangeState(player.moveState);
        }

        // ���ڸ� ���� �ִϸ��̼� ���� �� AnimationTrigger �߻� ������ ���� �ԷºҰ����ϰ� ����
        // AnimationTrigger �߻� �� �Է� �� state���� �����߻� attackStateTimer������ player���� ����
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

    public Vector3 PlayerToMousePosVec()
    {
        Vector2 MousePos = Input.mousePosition;
        MousePos = Camera.main.ScreenToWorldPoint(MousePos);
        Vector3 Pos = new Vector3(MousePos.x, MousePos.y, 0);
        Vector3 dir = Pos - player.transform.position;

        Vector3 dirNo = new Vector3(dir.x, dir.y, 0).normalized;
        
        return dirNo;
    }

    public void SetFinalAttkInputVec()
    {
        Vector2 finalAttkVecNormal = PlayerToMousePosVec();
        player.finalAttackInputVec = finalAttkVecNormal;
    }
}
