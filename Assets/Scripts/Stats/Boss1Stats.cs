using UnityEngine;
using UnityEngine.UI;

public class Boss1Stats : CharacterStats
{
    private Boss1 boss1 => GetComponent<Boss1>();

    private float hp;
    private float hp_Cur;

    [SerializeField] Image hpBar_Front;
    [SerializeField] Image hpBar_Back;


    protected override void Start()
    {
        base.Start();
        hp_Cur = (float)currentHealth;
        hp = (float)maxHealth.GetValue();
    }

    protected override void Update()
    {
        base.Update();
        SyncBar();

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

    private void SyncBar()
    {
        hp_Cur = (float)currentHealth;
        hpBar_Front.fillAmount = hp_Cur / hp;

        if (hpBar_Back.fillAmount > hpBar_Front.fillAmount)
        {
            hpBar_Back.fillAmount = Mathf.Lerp(hpBar_Back.fillAmount, hpBar_Front.fillAmount, Time.deltaTime);
        }
    }
}
