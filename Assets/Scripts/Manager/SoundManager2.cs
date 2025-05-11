using UnityEngine;
using UnityEngine.Audio;

public class SoundManager2 : MonoBehaviour
{
    public static SoundManager2 instance;

    public enum EBgm
    {
        Bgm_StageBattle,
        Bgm_BossBattle
    }

    public enum ESfx
    {
        SFX_SandeVistan,
        SFX_Santan_Bullet,
        SFX_CloseAttack,
        SFX_GrenadeExplosion,
        SFX_HurtSound,
        SFX_Boss1PowerOff
    }

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    public AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("사운드 매니저 시작됨");

        // 테스트용 강제 BGM 재생
        PlayBGM(EBgm.Bgm_StageBattle);

        Debug.Log($"볼륨: {audioBgm.volume}");
        Debug.Log($"클립 있음?: {audioBgm.clip != null}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (audioBgm.isPlaying)
            {
                audioBgm.Pause();
            }
            else
            {
                audioBgm.UnPause();
            }
        }

    }

    // EBgm 열거형을 매개변수로 받아 해당하는 배경 음악 클립을 재생
    public void PlayBGM(EBgm bgmIdx)
    {
        //enum int형으로 형변환 가능
        audioBgm.clip = bgms[(int)bgmIdx];
        audioBgm.Play();
    }

    // 현재 재생 중인 배경 음악 정지
    public void StopEBGM()
    {
        audioBgm.Stop();
    }

    // ESfx 열거형을 매개변수로 받아 해당하는 효과음 클립을 재생
    public void PlayESFX(ESfx esfx)
    {
        audioSfx.PlayOneShot(sfxs[(int)esfx]);
    }
}
