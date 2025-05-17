using System;
using System.Collections;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    private CapsuleCollider2D cd;
    private Boss1_AnimationTrigger anicontroller;
    private Animator ani;
    public EntityFX fx { get; private set; }
    public SpriteTrail MeshTrailscript { get; private set; }

    [Header("움직임 관련 변수들")]
    [SerializeField] private float speed = 5f;
    private float angle;
    private Vector3 dir;
    private int checkClosenum = 0;
    private int checkFirenum = 0;
    private int checkRocketnum = 0;
    private float playerToBossDistance;
    private bool isCoroutineRunning = false;
    private bool hasPowerOffExecuted = false;
    private bool Boss1Die = false;
    private bool BasicImplant = false;

    //상태 변환 관련
    private BossState currentState = BossState.Idle;
    private Boss1Stats boss1stats => GetComponent<Boss1Stats>();

    [Header("인스펙터 오브젝트")]
    [SerializeField] private GameObject inspectorObject;

    private void Start()
    {
        ani = GetComponent<Animator>();
        MeshTrailscript = ani.GetComponent<SpriteTrail>();
        anicontroller = GetComponent<Boss1_AnimationTrigger>();
        fx = GetComponent<EntityFX>();
        cd = GetComponent<CapsuleCollider2D>();
        StartCoroutine(HandleLayers());
    }

    private void Update()
    {
        CheckInput();
        if (!boss1stats.Engaging() && !boss1stats.EmptyHealth())
        {
            CheckDistance();
        }
        else if (boss1stats.Engaging())
        {
            Engaging();
        }
        AngleAnimation();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(SandeVistan());
        }
    }

    private IEnumerator SandeVistan()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Walk);
        MeshTrailscript.StartTrail();
        BasicImplant = true;
        Vector2 movement;
        Invoke("BasicImplantKids", 2f);
        ani.speed = 3f;

        while (BasicImplant)
        {
            movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.GetKey("Horizontal") || Input.GetKey("Vertical"))
            {
                anicontroller.rb.linearVelocity = movement * 5f;
            }
        }
        yield return null;
    }

    private void BasicImplantKids()
    {
        BasicImplant = false;
        isCoroutineRunning = false;
        ani.speed = 1f;
    }

    private void CheckDistance()
    {
        playerToBossDistance = Vector3.Distance(transform.position, anicontroller.player.position);

        if (playerToBossDistance < 3.5f && !isCoroutineRunning)
        {
            StartCoroutine(CloseAttack());
        }
        else if (playerToBossDistance > 3.5f && playerToBossDistance < 6f && !isCoroutineRunning && checkFirenum != 2)
        {
            StartCoroutine(ShotgunAttack());
        }
        else if (playerToBossDistance > 6f && playerToBossDistance < 8f && !isCoroutineRunning && checkRocketnum != 2)
        {
            StartCoroutine(RocketAttack());
        }
        else if (playerToBossDistance > 12f && playerToBossDistance < 30f && !isCoroutineRunning && checkClosenum == 0)
        {
            ChangeState(BossState.Walk);
        }
        else if (playerToBossDistance > 30f)
        {
            ChangeState(BossState.Idle);
        }
    }

    private void Engaging()
    {
        if (!hasPowerOffExecuted && boss1stats.Engaging() && !isCoroutineRunning)
        {
            hasPowerOffExecuted = true;
            StartCoroutine(PowerOff());
        }

        if (boss1stats.EmptyHealth() && !Boss1Die)
        {
            StartCoroutine(Boss1EmptyHealth());
        }

        if (Boss1Die)
        {
            speed = 0f;
        }
        else
        {
            speed = 10f;
        }

        playerToBossDistance = Vector3.Distance(transform.position, anicontroller.player.position);

        if (playerToBossDistance < 3.5f && !isCoroutineRunning)
        {
            StartCoroutine(CloseAttack());
        }
        else if (playerToBossDistance > 3.5f && playerToBossDistance < 6f && !isCoroutineRunning)
        {
            StartCoroutine(ShotgunAttack());
        }
        else if (playerToBossDistance > 6f && !isCoroutineRunning)
        {
            StartCoroutine(LanceAttack());
        }
    }

    private IEnumerator CloseAttack()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.CloseAttack);
        checkFirenum = 0;
        checkRocketnum = 0;
        checkClosenum++;
        yield return new WaitForSeconds(1.2f);
        isCoroutineRunning = false;
        ChangeState(BossState.Walk);
    }

    private IEnumerator ShotgunAttack()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Fire);
        checkFirenum++;
        yield return new WaitForSeconds(2f);
        ChangeState(BossState.Walk);
        isCoroutineRunning = false;
    }

    private IEnumerator RocketAttack()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Rocket);
        checkRocketnum++;
        yield return new WaitForSeconds(2.5f);
        ChangeState(BossState.Walk);
        isCoroutineRunning = false;
    }

    private IEnumerator LanceAttack()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Lancer);
        yield return new WaitForSeconds(3.5f);
        ChangeState(BossState.Walk);
        isCoroutineRunning = false;
    }

    public IEnumerator PowerOff()
    {
        isCoroutineRunning = true;
        cd.enabled = false;
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Boss1PowerOff);
        ChangeState(BossState.PowerOff);
        yield return new WaitForSeconds(5f);
        isCoroutineRunning = false;
        cd.enabled = true;
    }

    private IEnumerator Boss1EmptyHealth()
    {
        isCoroutineRunning = true;
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Boss1PowerOff);
        ChangeState(BossState.PowerOff);
        Boss1Die = true;
        yield return new WaitForSeconds(6f);
        if (inspectorObject != null)
        {
            inspectorObject.SetActive(true);
        }
    }

    #region Movement
    private IEnumerator HandleLayers()
    {
        while (true)
        {
            switch (currentState)
            {
                case BossState.Idle:
                    ActivateLayer(LayerName.IdleLayer);
                    anicontroller.SetZeroVelocity();
                    break;
                case BossState.Walk:
                    if (boss1stats.EmptyHealth())
                    {
                        StartCoroutine(Boss1EmptyHealth());
                        break;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, anicontroller.player.position, speed * Time.deltaTime);
                    ActivateLayer(LayerName.WalkLayer);
                    break;
                case BossState.Fire:
                    if (boss1stats.EmptyHealth())
                    {
                        StartCoroutine(Boss1EmptyHealth());
                        break;
                    }
                    ActivateLayer(LayerName.FireLayer);
                    break;
                case BossState.Rocket:
                    ActivateLayer(LayerName.RocketLayer);
                    break;
                case BossState.CloseAttack:
                    if (boss1stats.EmptyHealth())
                    {
                        StartCoroutine(Boss1EmptyHealth());
                        break;
                    }
                    ActivateLayer(LayerName.CloseAttackLayer);
                    break;
                case BossState.Lancer:
                    if (boss1stats.EmptyHealth())
                    {
                        StartCoroutine(Boss1EmptyHealth());
                        break;
                    }
                    ActivateLayer(LayerName.LancerLayer);
                    break;
                case BossState.PowerOff:
                    ActivateLayer(LayerName.PowerOffLayer);
                    anicontroller.SetZeroVelocity();
                    break;
                case BossState.EmptyHealth:
                    ActivateLayer(LayerName.PowerOffLayer);
                    anicontroller.SetZeroVelocity();
                    break;
            }
            yield return null;
        }
    }

    public void ChangeState(BossState newState)
    {
        currentState = newState;
    }
    #endregion

    #region Animation
    private void ActivateLayer(LayerName layerName)
    {
        for (int i = 0; i < ani.layerCount; i++)
        {
            ani.SetLayerWeight(i, 0);
        }
        ani.SetLayerWeight((int)layerName, 1);
    }

    public void AngleAnimation()
    {
        dir = anicontroller.player.position - transform.position;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle > -45 && angle <= 45)
        {
            ani.SetFloat("x", 1);
            ani.SetFloat("y", 0);
        }
        else if (angle > 45 && angle <= 135)
        {
            ani.SetFloat("x", 0);
            ani.SetFloat("y", 1);
        }
        else if (angle > 135 && angle <= 180 || angle <= -135)
        {
            ani.SetFloat("x", -1);
            ani.SetFloat("y", 0);
        }
        else if (angle > -135 && angle <= -45)
        {
            ani.SetFloat("x", 0);
            ani.SetFloat("y", -1);
        }
    }
    #endregion

    public void DamageEffect()
    {
        fx.StartCoroutine("FlashFX");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collision.GetComponent<PlayerStats>();
                Player player = collision.GetComponent<Player>();
                if (_target != null)
                {
                    _target.TakeDamage(30);
                    player.SetupKnockbackDir(gameObject.transform, 5f);
                }
            }
        }

        if (collision.CompareTag("Wall"))
        {
            collision.gameObject.GetComponent<GenerateWall>()?.BreakWall();
        }

        if (collision.CompareTag("StopWall") || collision.gameObject.layer == LayerMask.NameToLayer("StopWall"))
        {
            anicontroller.SetZeroVelocity();
        }
    }
}