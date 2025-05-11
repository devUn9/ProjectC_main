using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

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
        SFX_Boss1PowerOff,
        SFX_WallBreak,
        SFX_LazerBullet
    }

    [SerializeField] AudioClip[] bgms;
    [SerializeField] AudioClip[] sfxs;

    [SerializeField] AudioSource audioBgm;
    [SerializeField] AudioSource audioSfx;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(audioBgm.isPlaying)
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
