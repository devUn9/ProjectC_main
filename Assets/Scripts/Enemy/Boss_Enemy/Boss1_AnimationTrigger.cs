using System.Collections;
using UnityEngine;

public class Boss1_AnimationTrigger : MonoBehaviour
{
    public Transform player;
    public GameObject[] CloseAttackPoints; 
    public Rigidbody2D rb;

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
        CloseAttackPoints[0].SetActive(true);  
    }

    private void CloseAttackOutUp()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);  
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
    }

    private void CloseattackInDown()
    {
        CloseAttackPoints[1].SetActive(true);
    }

    private void CloseattackOutDown()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);  
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
    }
    private void CloseAttackInLeft()
    {
        CloseAttackPoints[2].SetActive(true);
    }

    private void CloseattackOutLeft()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);  
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
    }

    private void CloseAttackInRight()
    {
        CloseAttackPoints[3].SetActive(true);
    }
    private void CloseattackOutRight()
    {
        CloseAttackPoints[0].SetActive(false);
        CloseAttackPoints[1].SetActive(false);  
        CloseAttackPoints[2].SetActive(false);
        CloseAttackPoints[3].SetActive(false);
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
        while(lancing)
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
}
