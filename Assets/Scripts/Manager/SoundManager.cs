using UnityEngine;
using System;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private EndingCredit endingCredit; // 인스펙터에서 할당

    public enum EBgm
    {
        Bgm_StageBattle,
        Bgm_BossBattle,
        Bgm_EndingCredit,
        Bgm_City,
        Bgm_Enterprise,
        Bgm_MiniGame
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
        SFX_LazerBullet,
        SFX_EMPGrenadeExplosion,
        SFX_SmokeShellExplosion,
        SFX_LauncherArmExplosion,
        SFX_GravitonSurgeExplosion,
        SFX_SwordAttack,
        SFX_PlayerWalking,
        SFX_MonsterDie,
        SFX_RedBall,
        SFX_BlueBall,
        SFX_EnergyBall,
        SFX_EnergyShield,
        SFX_Clicker
    }

    [Serializable]
    public class BgmClip
    {
        public EBgm bgmType;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Serializable]
    public class SfxClip
    {
        public ESfx sfxType;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("BGM 설정")]
    [SerializeField] private List<BgmClip> bgmClips;

    [Header("SFX 설정")]
    [SerializeField] private List<SfxClip> sfxClips;

    [SerializeField] private AudioSource audioBgm;
    [SerializeField] private AudioSource audioSfx;
    [SerializeField] private AudioSource audioLoop;

    private Dictionary<EBgm, BgmClip> bgmDict;
    private Dictionary<ESfx, SfxClip> sfxDict;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 초기화
            bgmDict = new Dictionary<EBgm, BgmClip>();
            foreach (var bgm in bgmClips)
            {
                bgmDict[bgm.bgmType] = bgm;
            }

            sfxDict = new Dictionary<ESfx, SfxClip>();
            foreach (var sfx in sfxClips)
            {
                sfxDict[sfx.sfxType] = sfx;
            }
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
            if (audioBgm.isPlaying)
                audioBgm.Pause();
            else
                audioBgm.UnPause();
        }
    }

    public void PlayBGM(EBgm bgmType)
    {
        if (bgmDict.TryGetValue(bgmType, out BgmClip bgm))
        {
            audioBgm.clip = bgm.clip;
            audioBgm.volume = bgm.volume; // 소리 덮어씌워서 커져버림
            audioBgm.loop = true; // 루프 설정
            audioBgm.Play();
        }
    }

    public void StopEBGM()
    {
        audioBgm.Stop();
    }

    public void PlayESFX(ESfx sfxType)
    {
        if (endingCredit.isScrolling) return;
        if (sfxDict.TryGetValue(sfxType, out SfxClip sfx))
        {
            audioSfx.PlayOneShot(sfx.clip, sfx.volume);
        }
    }

    public void LoopESFX(ESfx sfxType)
    {
        if (endingCredit.isScrolling) return;
        if (sfxDict.TryGetValue(sfxType, out SfxClip sfx))
        {
            audioLoop.clip = sfx.clip;
            audioLoop.volume = sfx.volume;
            audioLoop.loop = true;
            audioLoop.Play();
        }
    }
}
