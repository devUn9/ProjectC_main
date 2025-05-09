using UnityEngine;

public class EnemySightTest : MonoBehaviour
{
    [SerializeField] private float alertRange = 5f;
    [SerializeField] private float detectRange = 2f;
    private GameObject _player;
    private EffectController sightEffect;
    private EnemySightState currentState = EnemySightState.Patrol;


    private enum EnemySightState
    {
        Patrol,
        Alert,
        Detected,
        Attack
    }

    void Start()
    {
        _player = PlayerManager.instance.player;

        sightEffect = EffectManager.Instance.PlayEffectFollow(EffectType.EnemySightEffect, transform, Quaternion.Euler(0, 0, -180f));

        UpdateState(EnemySightState.Patrol); 
    }

    void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        EnemySightState newState = SetState(distance);

        if (newState != currentState)
        {
            UpdateState(newState);
        }

    }

    private EnemySightState SetState(float distance)
    {
        if (distance <= detectRange)
            return EnemySightState.Detected;
        else if (distance <= alertRange)
            return EnemySightState.Alert;
        else
            return EnemySightState.Patrol;
    }

    private void UpdateState(EnemySightState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case EnemySightState.Patrol:
                sightEffect.SetSightEffect(5f, Quaternion.Euler(0, 0, -180f), 90f);
                sightEffect.SetSightColor(Color.yellow);
                break;

            case EnemySightState.Alert:
                sightEffect.SetSightEffect(7f, Quaternion.Euler(0, 0, -180f), 90f);
                sightEffect.SetSightColor(Color.red);
                break;

            case EnemySightState.Detected:
                sightEffect.SetSightEffect(0f, Quaternion.Euler(0, 0, -180f), 0f);
                sightEffect.SetSightColor(Color.yellow);
                break;

        }

    }



}


