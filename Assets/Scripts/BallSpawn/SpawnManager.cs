using NUnit.Framework;
using System.Collections;
using System.Linq;
using UnityEngine;

enum BallType
{
    DamageBall,
    EnergyBall
}

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private BallType ballType;

    [SerializeField] private Transform[] spawnPoint;

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
        if (!spawnTrigger && !isRespawn)
            return;

        if (ballType == BallType.EnergyBall)
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

        //randomBallIndex = Random.Range(0, 1);
        //Instantiate(damageBallPrefabs[0], spawnPoint[Index + 4]);

        yield return new WaitForSeconds(respawnDuration);

        isRespawn = true;
    }

    IEnumerator EnergyBallSpawning()
    {
        isRespawn = false;

        int randomIndex = Random.Range(0, spawnPoint.Length);
        int Index = randomIndex % spawnPoint.Length;

        Instantiate(energyPrefab, spawnPoint[Index]);

        yield return new WaitForSeconds(energyRespawnDuration);

        isRespawn = true;
    }

}
