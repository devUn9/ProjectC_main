using System.Collections;
using UnityEngine;

public enum BossState
{
    Idle,
    Walk,
    Pattern1,
    Pattern2,
    Enraged,
    Dead
}

public class Boss1_Pattern : MonoBehaviour
{
    private BossState currentState = BossState.Idle;

    void Start()
    {
        StartCoroutine(StateMachine());
    }
    
    void Update()
    {
        
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case BossState.Idle:
                    HandleIdle();
                    break;

                case BossState.Walk:
                    HandleWalk();
                    break;

                case BossState.Pattern1:
                    HandlePattern1();
                    break;

                case BossState.Pattern2:
                    HandlePattern2();
                    break;

                case BossState.Enraged:
                    HandleEnraged();
                    break;

                case BossState.Dead:
                    HandleDead();
                    yield break; 
            }

            yield return null; 
        }
    }

    private void HandleIdle()
    {
        
    }

    private void HandleWalk()
    {

    }

    private void HandlePattern1()
    {

    }

    private void HandlePattern2()
    {

    }

    private void HandleEnraged()
    {

    }

    private void HandleDead()
    {
        
    }

    public void ChangeState(BossState newState)
    {
        currentState = newState;
    }

}
