using UnityEngine;
using UnityEngine.Audio;

public class SoundManager3 : MonoBehaviour
{
    public static SoundManager3 instance;

    public enum EBgm
    {
        Bgm_StageBattle,
        Bgm_BossBattle,
        Bgm_City,
        Bgm_Enterprise,
        Bgm_Ending
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

    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] sfxs;
    [SerializeField] private AudioSource audioBgm;
    [SerializeField] private AudioSource audioSfx;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SoundManager initialized as singleton", this);
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate SoundManager destroyed", this);
            return;
        }

        // null 체크
        if (audioBgm == null || audioSfx == null)
        {
            Debug.LogError("AudioBgm or AudioSfx is not assigned", this);
        }
        if (bgms == null || bgms.Length < (int)EBgm.Bgm_Ending + 1)
        {
            Debug.LogError("bgms array is not properly assigned or has insufficient clips", this);
        }
        else
        {
            for (int i = 0; i < bgms.Length; i++)
            {
                if (bgms[i] == null)
                {
                    Debug.LogError($"BGM clip at index {i} is null", this);
                }
            }
        }
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

    public void PlayBGM(EBgm bgmIdx)
    {
        int index = (int)bgmIdx;
        if (index < 0 || index >= bgms.Length || bgms[index] == null)
        {
            Debug.LogError($"Cannot play BGM {bgmIdx}: Invalid index or null clip", this);
            return;
        }

        audioBgm.clip = bgms[index];
        audioBgm.Play();
    }

    public void StopEBGM()
    {
        audioBgm.Stop();
    }

    public void PlayESFX(ESfx esfx)
    {
        int index = (int)esfx;
        if (index < 0 || index >= sfxs.Length || sfxs[index] == null)
        {
            Debug.LogError($"Cannot play SFX {esfx}: Invalid index or null clip", this);
            return;
        }

        audioSfx.PlayOneShot(sfxs[index]);
    }
}