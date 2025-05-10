using UnityEngine;

public class PlayerSightTest : MonoBehaviour
{

    private EffectController sightEffect;
 

    void Start()
    {

        sightEffect = EffectManager.Instance.PlayEffectFollow(EffectType.EnemySightEffect, transform, Quaternion.Euler(0, 0, -180f));
        sightEffect.SetSightEffect(7f, Quaternion.Euler(0, 0, -180f), 360f);
        sightEffect.SetSightColor(Color.yellow);

    }

    void Update()
    {



    }

    



}


