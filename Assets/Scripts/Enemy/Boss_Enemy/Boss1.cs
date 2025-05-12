using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Boss1 : MonoBehaviour
{
    private BossState currentState = BossState.Idle;
    private Boss1_AnimationTrigger anicontroller;
    public CharacterStats stats { get; private set; }

    public Transform player;
    private Animator ani;
    public EntityFX fx { get; private set; }

    private Boss1Stats boss1stats => GetComponent<Boss1Stats>();

    public SpriteTrail MeshTrailscript { get; private set; }

    [Header("움직임 관련 변수들")]
    [SerializeField] private float speed = 5f;
    private float angle;
    private Vector3 dir;

    [Header("공격관련 컴포넌트")]
    public GameObject[] FirePoints;
    public GameObject bulletPrefab;
    public GameObject[] ExplorePoints;
    public GameObject explorePrefab;

    [Header("인스펙터 오브젝트")]
    [SerializeField] private GameObject inspectorObject;

    //몬스터 패턴 관련 변수들
    private float playerToBossDistance;
    private bool isCoroutineRunning = false;

    // PowerOff 실행 여부를 확인하는 플래그
    private bool hasPowerOffExecuted = false;
    private bool Boss1Die = false;

    private void Start()
    {
        ani = GetComponent<Animator>();
        MeshTrailscript = ani.GetComponent<SpriteTrail>();
        anicontroller = GetComponent<Boss1_AnimationTrigger>();
        fx = GetComponent<EntityFX>();
        StartCoroutine(HandleLayers());
        StartCoroutine(MeetPattern());
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
        MeshTrailscript.StartTrail();
        yield return new WaitForSeconds(1f);
    }

    private void CheckDistance()
    {
        playerToBossDistance = Vector3.Distance(transform.position, player.position);

        if (playerToBossDistance < 3.5f && !isCoroutineRunning)
        {
            StartCoroutine(CloseAttack());
        }
        else if (playerToBossDistance > 3.5f && playerToBossDistance < 6f && !isCoroutineRunning)
        {
            StartCoroutine(ShotgunAttack());
        }
        else if (playerToBossDistance > 6f && playerToBossDistance < 8f && !isCoroutineRunning)
        {
            StartCoroutine(RocketAttack());
        }
        else if (playerToBossDistance > 12f && playerToBossDistance < 30f && !isCoroutineRunning)
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

        playerToBossDistance = Vector3.Distance(transform.position, player.position);

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
        yield return new WaitForSeconds(1.2f);
        isCoroutineRunning = false;
        ChangeState(BossState.Walk);
    }

    private IEnumerator ShotgunAttack()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Fire);
        yield return new WaitForSeconds(2f);
        ChangeState(BossState.Walk);
        isCoroutineRunning = false;
    }

    private IEnumerator RocketAttack()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Rocket);
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
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Boss1PowerOff);
        ChangeState(BossState.PowerOff);
        yield return new WaitForSeconds(5f);
        isCoroutineRunning = false;
    }

    private IEnumerator Boss1EmptyHealth()
    {
        isCoroutineRunning = true;
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Boss1PowerOff);
        ChangeState(BossState.PowerOff);
        Boss1Die = true;
        if (inspectorObject != null)
        {
            inspectorObject.SetActive(true);
        }
        yield return new WaitForSeconds(99f);
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
                    transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                    ActivateLayer(LayerName.WalkLayer);
                    break;
                case BossState.Fire:
                    ActivateLayer(LayerName.FireLayer);
                    break;
                case BossState.Rocket:
                    ActivateLayer(LayerName.RocketLayer);
                    break;
                case BossState.CloseAttack:
                    ActivateLayer(LayerName.CloseAttackLayer);
                    break;
                case BossState.Lancer:
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
        if (Boss1Die)
        {
            currentState = BossState.PowerOff;
            return;
        }
        currentState = newState;
    }

    public IEnumerator MeetPattern()
    {
        isCoroutineRunning = true;
        ChangeState(BossState.Lancer);
        yield return new WaitForSeconds(3.5f);
        ChangeState(BossState.Idle);
        anicontroller.LancerOutRight();
        yield return new WaitForSeconds(0.1f);
        ChangeState(BossState.Walk);
        isCoroutineRunning = false;
    }
    #endregion

    #region Attack
    private void SantanInUp()
    {
        FirePoints[0].SetActive(true);
        int count = 20;
        float intervalAngle = 90f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[0].transform.position, Quaternion.identity);
            float angle = 45f + (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Santan_Bullet);
    }

    private void SantanOutUp()
    {
        FirePoints[0].SetActive(false);
    }

    private void SantanInDown()
    {
        FirePoints[1].SetActive(true);
        int count = 20;
        float intervalAngle = 90f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[1].transform.position, Quaternion.identity);
            float angle = -45f - (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Santan_Bullet);
    }

    private void SantanOutDown()
    {
        FirePoints[1].SetActive(false);
    }

    private void SantanInLeft()
    {
        FirePoints[2].SetActive(true);
        int count = 20;
        float intervalAngle = 90f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[2].transform.position, Quaternion.identity);
            float angle = 135f + (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Santan_Bullet);
    }

    private void SantanOutLeft()
    {
        FirePoints[2].SetActive(false);
    }

    private void SantanInRight()
    {
        FirePoints[3].SetActive(true);
        int count = 20;
        float intervalAngle = 90f / (count - 1);
        for (int i = 0; i < count; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, FirePoints[3].transform.position, Quaternion.identity);
            float angle = -45f + (i * intervalAngle);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = Mathf.Sin(angle * Mathf.Deg2Rad);
            clone.GetComponent<Santan_Bullet>().Move(new Vector2(x, y));
        }
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_Santan_Bullet);
    }

    private void SantanOutRight()
    {
        FirePoints[3].SetActive(false);
    }

    private void RocketInUp()
    {
        ExplorePoints[0].SetActive(true);
        Instantiate(explorePrefab, ExplorePoints[0].transform.position, Quaternion.identity);
        ExplorePoints[0].SetActive(false);
    }

    private void RocketInDown()
    {
        ExplorePoints[1].SetActive(true);
        Instantiate(explorePrefab, ExplorePoints[1].transform.position, Quaternion.identity);
        ExplorePoints[1].SetActive(false);
    }

    private void RocketInLeft()
    {
        ExplorePoints[2].SetActive(true);
        Instantiate(explorePrefab, ExplorePoints[2].transform.position, Quaternion.identity);
        ExplorePoints[2].SetActive(false);
    }

    private void RocketInRight()
    {
        ExplorePoints[3].SetActive(true);
        Instantiate(explorePrefab, ExplorePoints[3].transform.position, Quaternion.identity);
        ExplorePoints[3].SetActive(false);
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
        dir = player.position - transform.position;
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
                    player.SetupKnockbackDir(gameObject.transform, 10f);
                    SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_HurtSound);
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