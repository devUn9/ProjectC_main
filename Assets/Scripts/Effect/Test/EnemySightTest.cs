using UnityEngine;

public class EnemySightTest : MonoBehaviour
{
    [SerializeField] private float alertRange = 15f;
    [SerializeField] private float detectRange = 5f;
    private GameObject _player;
    private EffectController sightEffect;
    private EnemySightState currentState = EnemySightState.Patrol;

    private Vector3 targetScale;
    private Color targetColor;
    private SpriteRenderer effectRenderer; 

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
        sightEffect = EffectManager.Instance.PlayEffectFollow(EffectType.EnemySightEffect, transform, new Vector3(0f, -1.2f, 0f));

        effectRenderer = sightEffect.GetComponentInChildren<SpriteRenderer>();

        UpdateState(EnemySightState.Patrol, true); 
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

        if (sightEffect != null)
        {
            sightEffect.transform.localScale = Vector3.Lerp(sightEffect.transform.localScale, targetScale, Time.deltaTime * 5f);

            if (effectRenderer != null)
            {
                effectRenderer.color = Color.Lerp(effectRenderer.color, targetColor, Time.deltaTime * 5f);
            }
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

    private void UpdateState(EnemySightState newState, bool instant = false)
    {
        currentState = newState;

        switch (currentState)
        {
            case EnemySightState.Patrol:
                targetScale = Vector3.one * 1f; // 순찰 크기 작음
                targetColor = new Color(1f, 0.5f, 0f, 0.5f); // 주황색, 반투명
                break;

            case EnemySightState.Alert:
                targetScale = Vector3.one * 2.0f; // 경계 크기 큼
                targetColor = new Color(1f, 0.5f, 0f, 0.7f); // 주황색, 더 진하게
                break;

            case EnemySightState.Detected:
                targetScale = Vector3.one * 2.0f; // 발견 크기 큼
                targetColor = new Color(1f, 0f, 0f, 0.7f); // 빨강색
                break;

            case EnemySightState.Attack:
                sightEffect?.DestroyEffect(); // 이펙트 삭제
                sightEffect = null;
                return;
        }

        if (instant && sightEffect != null)
        {
            // 초기화 때는 즉시 적용
            sightEffect.transform.localScale = targetScale;
            if (effectRenderer != null)
                effectRenderer.color = targetColor;
        }
    }

    public void EnterAttackState()
    {
        UpdateState(EnemySightState.Attack);
    }
}
