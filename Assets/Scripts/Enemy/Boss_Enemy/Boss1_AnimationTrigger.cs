using System.Collections;
using UnityEditor;
using UnityEngine;

public class Boss1_AnimationTrigger : MonoBehaviour
{
    private Boss1 boss1 => GetComponent<Boss1>();
    public Transform player;
    public GameObject[] CloseAttackPoints;
    public float AttackCheckRadius = 2f;
    public Rigidbody2D rb;
    public GameObject CloseAttackEffectPrefab;


    private bool lancing = false;
    private float LancingPower = 0.05f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

    }

    private void CloseAttackInUp()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[0].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        //SoundManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[0].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Health_Player>() != null)
            {
                Health_Player _target = collider.GetComponent<Health_Player>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(30f);
                    player.SetupKnockbackDir(gameObject.transform);
                }
            }
        }
    }

    private void CloseattackInDown()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[1].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        //SoundManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[1].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Health_Player>() != null)
            {
                Health_Player _target = collider.GetComponent<Health_Player>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(30f);
                    player.SetupKnockbackDir(gameObject.transform);
                }
            }
        }
    }

    private void CloseAttackInLeft()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[2].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        //SoundManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[2].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Health_Player>() != null)
            {
                Health_Player _target = collider.GetComponent<Health_Player>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(30f);
                    player.SetupKnockbackDir(gameObject.transform);
                }
            }
        }
    }

    private void CloseAttackInRight()
    {
        GameObject effect = Instantiate(CloseAttackEffectPrefab, CloseAttackPoints[3].transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        //SoundManager.instance.PlaySFX(2, null);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(CloseAttackPoints[3].transform.position, AttackCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Health_Player>() != null)
            {
                Health_Player _target = collider.GetComponent<Health_Player>();
                Player player = collider.GetComponent<Player>();

                if (_target != null)
                {
                    _target.TakeDamage(30f);
                    player.SetupKnockbackDir(gameObject.transform);
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
        while (lancing)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.AddForce(direction * LancingPower, ForceMode2D.Impulse);
            yield return null;
        }
    }

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
