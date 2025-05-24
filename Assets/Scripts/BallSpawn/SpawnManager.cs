using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoint;

    [SerializeField] private BallType spawnType;

    [Header("DamageBall Info")]
    [SerializeField] private GameObject[] damageBallPrefabs;
    [SerializeField] private float respawnDuration;
    [SerializeField] private float respawnPointNumber;

    [Header("Energy Info")]
    [SerializeField] private GameObject energyPrefab;
    [SerializeField] private float energyRespawnDuration;
    [SerializeField] private float energyRespawnPointNumber;

    public bool spawnTrigger = false;
    [SerializeField] private bool isRespawn = true;

    void Start()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        spawnPoint = spawnPoint.Skip(1).ToArray(); // 첫 번째(자기 자신) 제외
    }

    void Update()
    {
        if (!spawnTrigger)
            return;
        if (!isRespawn)
            return;

        if (spawnType == BallType.EnergyBall)
        {
            StartCoroutine(EnergyBallSpawning());
        }
        else
        {
            StartCoroutine(DamageBallSpawning());
        }

    }

    IEnumerator DamageBallSpawning()
    {
        isRespawn = false;

        int randomIndex = Random.Range(0, spawnPoint.Length);
        int Index = randomIndex % spawnPoint.Length;

        int randomBallIndex = Random.Range(0, 2);
        int ballIndex = randomBallIndex % damageBallPrefabs.Length;

        Instantiate(damageBallPrefabs[ballIndex], spawnPoint[Index]);

        yield return new WaitForSeconds(respawnDuration);

        isRespawn = true;
    }

    IEnumerator EnergyBallSpawning()
    {
        isRespawn = false;

        int maxTry = spawnPoint.Length;
        int tryCount = 0;
        int Index = -1;

        // 빈 spawnPoint를 찾을 때까지 반복
        while (tryCount < maxTry)
        {
            int randomIndex = Random.Range(0, spawnPoint.Length);
            Index = randomIndex % spawnPoint.Length;

            // 해당 spawnPoint에 energyPrefab이 이미 있는지 검사
            bool hasEnergy = false;
            foreach (Transform child in spawnPoint[Index])
            {
                if (child.CompareTag("EnergyBall"))
                {
                    hasEnergy = true;
                    break;
                }
            }

            if (!hasEnergy)
                break; // 빈 자리 발견

            tryCount++;
        }

        // 빈 자리가 있으면 스폰
        if (Index != -1)
        {
            GameObject obj = Instantiate(energyPrefab, spawnPoint[Index]);
            obj.tag = "EnergyBall";
        }

        yield return new WaitForSeconds(energyRespawnDuration);

        isRespawn = true;
    }

}
