using System.Collections;
using UnityEngine;

public class Health_Boss1 : Health_Entity
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override float MaxHP => 100f;
    public override float HPRecovery => 0f;

    public override void TakeDamage(float damage)
    {
        HP -= damage;
        StartCoroutine("Hit");
    }

    private IEnumerator Hit()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = original;
    }
}
