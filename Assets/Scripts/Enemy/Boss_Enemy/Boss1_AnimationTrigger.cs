using System.Collections;
using UnityEngine;

public class Boss1_AnimationTrigger : MonoBehaviour
{
    private Boss1Stats boss1stats => GetComponent<Boss1Stats>();
    public Transform player { get; private set; }
    public Rigidbody2D rb { get; private set; }

    [Header("공격관련 컴포넌트")]
    [SerializeField] private GameObject[] FirePoints;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject[] ExplorePoints;
    [SerializeField] private GameObject explorePrefab;
    [SerializeField] private GameObject[] CloseAttackPoints;
    [SerializeField] private GameObject CloseAttackEffectPrefab;

    [Header("공격관련 컴포넌트")]
    private float AttackCheckRadius = 1f;
    private int CloseAttackDamage = 30;
    private float closeattackknockbackforce = 20f;
    private bool lancing = false;
    private float LancingPower = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

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

    private void CloseAttackInUp()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[0].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_CloseAttack);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[0].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collider.GetComponent<PlayerStats>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(CloseAttackDamage);
                    player.SetupKnockbackDir(gameObject.transform, closeattackknockbackforce);
                }
            }
        }
    }

    private void CloseattackInDown()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[1].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_CloseAttack);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[1].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collider.GetComponent<PlayerStats>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(CloseAttackDamage);
                    player.SetupKnockbackDir(gameObject.transform, closeattackknockbackforce);
                }
            }
        }
    }

    private void CloseAttackInLeft()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[2].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_CloseAttack);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[2].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collider.GetComponent<PlayerStats>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(CloseAttackDamage);
                    player.SetupKnockbackDir(gameObject.transform, closeattackknockbackforce);
                }
            }
        }
    }

    private void CloseAttackInRight()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[3].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_CloseAttack);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[3].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<PlayerStats>() != null)
            {
                PlayerStats _target = collider.GetComponent<PlayerStats>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(CloseAttackDamage);
                    player.SetupKnockbackDir(gameObject.transform, closeattackknockbackforce);
                }
            }
        }
    }

    private void LancerInUp()
    {
        CloseAttackPoints[0].SetActive(true);
        lancing = true;
        StartCoroutine(Lancing());
    }

    private void LancerOutUp()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
        lancing = false;
        StopCoroutine(Lancing());
        SetZeroVelocity();
    }

    private void LancerInDown()
    {
        CloseAttackPoints[1].SetActive(true);
        lancing = true;
        StartCoroutine(Lancing());
    }

    private void LancerOutDown()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
        lancing = false;
        StopCoroutine(Lancing());
        SetZeroVelocity();
    }

    private void LancerInLeft()
    {
        CloseAttackPoints[2].SetActive(true);
        lancing = true;
        StartCoroutine(Lancing());
    }

    private void LancerOutLeft()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
        lancing = false;
        StopCoroutine(Lancing());
        SetZeroVelocity();
    }

    private void LancerInRight()
    {
        CloseAttackPoints[3].SetActive(true);
        lancing = true;
        StartCoroutine(Lancing());
    }

    public void LancerOutRight()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
        lancing = false;
        StopCoroutine(Lancing());
        SetZeroVelocity();
    }

    private IEnumerator Lancing()
    {
        if (boss1stats.Engaging())
        {
            LancingPower = 0.3f;
        }

        if (boss1stats.EmptyHealth())
        {
            yield return null;
        }

        SoundManager.instance.PlayESFX(SoundManager.ESfx.SFX_SandeVistan);
        while (lancing)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.AddForce(direction * LancingPower, ForceMode2D.Impulse);
            yield return null;
        }
    }
    #endregion

    public void SetZeroVelocity()
    {
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        foreach (GameObject point in CloseAttackPoints)
        {
            Gizmos.DrawWireSphere(point.transform.position, AttackCheckRadius);
        }
    }
}
