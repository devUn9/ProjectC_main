using System;
using System.Collections;
using UnityEngine;

public class MinigameObject : MonoBehaviour
{
    public int HP;
    public int MaxHP;

    private int interactionPoint;

    [SerializeField] private SpawnManager damageBallSpawn;
    [SerializeField] private SpawnManager energyBallSpawn;

    [SerializeField] private GameObject targetTrigger;
    [SerializeField] private GameObject targetTrigger2;
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
        hpUI.gameObject.SetActive(true);
    }

    private void TriggerOff()
    {
        damageBallSpawn.spawnTrigger = false;
        energyBallSpawn.spawnTrigger = false;
        
    }

    public void MiniGameClear()
    {
        // 코루틴 시작
        StartCoroutine(ActivateTriggersWithDelay());
    }

    private IEnumerator ActivateTriggersWithDelay()
    {
        hpUI.gameObject.SetActive(false);
        // 3초 뒤 targetTrigger 추가 작업 (예: 재활성화)
        yield return new WaitForSeconds(5f);
        if (targetTrigger != null)
        {
            targetTrigger.SetActive(true);
            Debug.Log("targetTrigger 3초 후 활성화");
        }

        // 5초 뒤 targetTrigger2 활성화
        yield return new WaitForSeconds(1f);
        if (targetTrigger2 != null)
        {
            targetTrigger2.SetActive(true);
            Debug.Log("targetTrigger2 8초 후 활성화");
        }
    }

    private void GameOver()
    {

        Debug.Log("Game Over!!");
    }
}
