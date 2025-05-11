using System.Collections;
using UnityEngine;

public enum BossState
{
    Idle,
    Walk,
    Fire,
    Rocket,
    CloseAttack,
    Lancer,
    PowerOff
}

public enum LayerName
{
    IdleLayer = 0,
    WalkLayer = 1,
    FireLayer = 2,
    RocketLayer = 3,
    CloseAttackLayer = 4,
    LancerLayer = 5,
    PowerOffLayer = 6,
}


public class Boss1_Pattern : MonoBehaviour
{
}
