using UnityEngine;

public class Boss1Stats : CharacterStats
{
    private Boss1 boss1;

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();


    }

    public bool Engaging()
    {
        if(currentHealth < maxHealth.GetValue()/2)
            return true;
        
        return false;
    }

    
}
