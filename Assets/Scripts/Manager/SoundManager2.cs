using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager2 : MonoBehaviour
{
    public static SoundManager2 instance;

    public enum EBgm
    {
        Bgm_Start,
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

        //// 테스트용 강제 BGM 재생
        //PlayBGM(EBgm.Bgm_StageBattle);

        Debug.Log($"볼륨: {audioBgm.volume}");
        Debug.Log($"클립 있음?: {audioBgm.clip != null}");

        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬 이름으로 초기 BGM 설정
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"[사운드매니저] 초기 씬: {sceneName}");

        PlayBgmDelayedByScene(sceneName);

        //// 현재 씬에 따라 초기 BGM 재생
        //if (sceneName.Contains("GameStart"))
        //{
        //    PlayBGM(EBgm.Bgm_Start);
        //}
        //else if (sceneName.Contains("Hook"))
        //{
        //    PlayBGM(EBgm.Bgm_StageBattle);
        //}
        //else
        //{
        //    PlayBGM(EBgm.Bgm_BossBattle);
        //}
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        Debug.Log($"[사운드매니저] 씬 로드됨: {sceneName}");

        PlayBgmDelayedByScene(sceneName);
    }

    private void PlayBgmDelayedByScene(string sceneName)
    {
        StopAllCoroutines();      // 중복 방지
        audioBgm.Stop();          // 기존 BGM 즉시 정지

        if (sceneName.Contains("GameStart"))
        {
            // GameStart 씬만 5초 후 재생
            StartCoroutine(PlayBgmWithDelay(EBgm.Bgm_Start, 5f));
        }
        else if (sceneName.Contains("Hook"))
        {
            PlayBGM(EBgm.Bgm_StageBattle); // 즉시 재생
        }
        else
        {
            PlayBGM(EBgm.Bgm_BossBattle); // 즉시 재생
        }
    }


    private IEnumerator PlayBgmWithDelay(EBgm bgmType, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBGM(bgmType);
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
