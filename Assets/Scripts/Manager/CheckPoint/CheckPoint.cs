using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Header("체크포인트 설정")]
    public bool isActivated = false;
    public GameObject activatedEffect;
    public AudioClip checkpointSound;

    private AudioSource audioSource;
    private Renderer checkpointRenderer;

    public Portal3 portalInfo;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        checkpointRenderer = GetComponent<Renderer>();

        // 비활성화 상태 표시
        if (checkpointRenderer != null && !isActivated)
            checkpointRenderer.material.color = Color.gray;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            Debug.Log("접촉");
            ActivateCheckpoint();
        }
    }

    // 2D용
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        isActivated = true;

        // 체크포인트 매니저에 등록
        CheckpointManager.Instance.SetCheckpoint(transform, portalInfo);

        //// 시각적 효과
        //if (checkpointRenderer != null)
        //    checkpointRenderer.material.color = Color.green;

        if (activatedEffect != null)
        {
            GameObject effect = Instantiate(activatedEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        //// 사운드 재생
        //if (checkpointSound != null && audioSource != null)
        //    audioSource.PlayOneShot(checkpointSound);

        Debug.Log("체크포인트 활성화: " + gameObject.name);
    }
}
