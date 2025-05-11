using UnityEngine;

public class Boss1Stats : CharacterStats
{
    private Boss1 boss1 => GetComponent<Boss1>();

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();


    }
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        boss1.DamageEffect();
    }

    public bool Engaging()
    {
        if (currentHealth < maxHealth.GetValue() / 2 && currentHealth > 0)
        {
            return true;
        }
        return false;
    }

    public bool EmptyHealth()
    {
        if (currentHealth < 0)
        {
            return true;
        }

        return false;
    }


}
