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

    // 키 입력 상태 관리를 위한 변수들
    protected bool isKeyProcessing = false;   // 현재 키 처리 중인지 여부
    protected GrenadeSkill skill;
    protected Camera mainCamera;

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
        player.attackStatusRemainTime -= Time.deltaTime;
        Debug.Log(animBoolName);

        stateInputVec.x = Input.GetAxisRaw("Horizontal");
        stateInputVec.y = Input.GetAxisRaw("Vertical");
        stateInputVec = stateInputVec.normalized;
        //이동 사격 시 trigger작동을 위한 조건문
        //이동 사격 state로직 마지막에 두면 다른 애니매이션 동작 중간에 다른 state로 변경되어 trigger 발동 하지않음
        if (triggerCalled && player.isMovingAttack)
        {
            player.isMovingAttack = false;
            Debug.Log("MoveAttackEnd");
            stateMachine.ChangeState(player.idleState);
        }

        if (triggerCalled && player.isDaggerAttack)
        {
            player.isDaggerAttack = false;
            Debug.Log("DaggerAttackEnd");
            stateMachine.ChangeState(player.idleState);
        }

        if (animBoolName != "Idle")
        {
            if (player.attackStateTimer > 0 && player.attackStatusRemainTime > 0)
            {
                PlayerToMousePosDir();
            }
            else if(player.attackStateTimer > 0 && player.isDaggerAttack)
            {
                SetAnimDirection(player.lastDirection);
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

        // 제자리 공격 애니매이션 종료 후 AnimationTrigger 발생 전까지 공격 입력불가능하게 조정
        // AnimationTrigger 발생 전 입력 시 state에서 에러발생 attackStateTimer변수는 player에서 관리
        if (Input.GetKeyDown(KeyCode.Mouse0) && player.attackStateTimer < 0.25)
        {
            player.Interaction();
            if (player.isDaggerAttack)
            {
                stateMachine.ChangeState(player.daggerAttack);
            }
            else if (animBoolName == "Move" && !player.isDaggerAttack)
                stateMachine.ChangeState(player.pistolMove);
            else
                stateMachine.ChangeState(player.attackState);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)
            ||Input.GetKeyDown(KeyCode.Alpha2)
            ||Input.GetKeyDown(KeyCode.Alpha3))
        {
            stateMachine.ChangeState(player.grenadeSkill);
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

    public string CurrentStateRecord()
    {
        return player.beforeState = this.animBoolName;
    }

    
}
