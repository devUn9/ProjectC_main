using System;
using UnityEngine;

public class MinigameObject : MonoBehaviour
{
    public int HP;
    public int MaxHP;

    private int interactionPoint;

    [SerializeField] private SpawnManager damageBallSpawn;
    [SerializeField] private SpawnManager energyBallSpawn;

    [SerializeField] private GameObject targetTrigger;
    [SerializeField] private MinigameHPUI hpUI; // UI 스크립트 참조

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Ball>())
        {
            Ball ball = collision.GetComponent<Ball>();
            interactionPoint = ball.damage;
            BallInteraction(interactionPoint);

            Destroy(ball.gameObject);
        }
    }

    private void BallInteraction(int _interactionPoint)
    {
        // UI 갱신
        if (hpUI != null)
        {
            hpUI.UpdateHPUI();
        }

        if (HP > MaxHP)
        {
            TriggerOff();
            MiniGameClear();
        }
        else if (HP <= 0)
        {
            GameOver();
        }
        HP -= _interactionPoint;
    }

    public void TriggerOn()
    {
        damageBallSpawn.spawnTrigger = true;
        energyBallSpawn.spawnTrigger = true;
    }

    private void TriggerOff()
    {
        damageBallSpawn.spawnTrigger = false;
        energyBallSpawn.spawnTrigger = false;
    }

    public void MiniGameClear()
    {
        targetTrigger.SetActive(true);
    }

    private void GameOver()
    {

        Debug.Log("Game Over!!");
    }
}
