using UnityEngine;

public class PlayerBGMController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // 플레이어의 Transform
    private SoundManager.EBgm currentBGM; // 현재 재생 중인 BGM 추적

    private void Awake()
    {
        if (playerTransform == null)
        {
            playerTransform = GetComponent<Transform>();
            if (playerTransform == null)
            {
                Debug.LogError("Player Transform is not assigned and could not be found on " + gameObject.name, this);
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null || SoundManager.instance == null)
        {
            Debug.LogWarning("Player Transform or SoundManager is null", this);
            return;
        }

        float yPosition = playerTransform.position.y;
        SoundManager.EBgm targetBGM;

        // Y축 위치에 따라 BGM 선택
        if (yPosition <= 180f)
        {
            targetBGM = SoundManager.EBgm.Bgm_StageBattle; // BGM 1번
        }
        else if (yPosition > 180f && yPosition <= 220f)
        {
            targetBGM = SoundManager.EBgm.Bgm_City; // BGM 2번 (가정)
        }
        else if (yPosition > 220f && yPosition <= 330f)
        {
            targetBGM = SoundManager.EBgm.Bgm_Enterprise; // BGM 3번 (가정)
        }
        else if (yPosition >= 330f)
        {
            targetBGM = SoundManager.EBgm.Bgm_BossBattle; // BGM 3번 (가정)
        }
        else
        {
            targetBGM = SoundManager.EBgm.Bgm_Ending;
        }

        // 현재 BGM과 다를 경우에만 재생
        if (currentBGM != targetBGM)
        {
            currentBGM = targetBGM;
            SoundManager.instance.PlayBGM(currentBGM);
            Debug.Log($"Playing BGM: {currentBGM} at Y position: {yPosition}", this);
        }
    }
}