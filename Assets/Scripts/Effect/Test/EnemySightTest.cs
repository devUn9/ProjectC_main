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
                targetScale = Vector3.one * 1f; // ���� ũ�� ����
                targetColor = new Color(1f, 0.5f, 0f, 0.5f); // ��Ȳ��, ������
                break;

            case EnemySightState.Alert:
                targetScale = Vector3.one * 2.0f; // ��� ũ�� ŭ
                targetColor = new Color(1f, 0.5f, 0f, 0.7f); // ��Ȳ��, �� ���ϰ�
                break;

            case EnemySightState.Detected:
                targetScale = Vector3.one * 2.0f; // �߰� ũ�� ŭ
                targetColor = new Color(1f, 0f, 0f, 0.7f); // ������
                break;

            case EnemySightState.Attack:
                sightEffect?.DestroyEffect(); // ����Ʈ ����
                sightEffect = null;
                return;
        }

        if (instant && sightEffect != null)
        {
            // �ʱ�ȭ ���� ��� ����
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
