using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle = 0,   // 아무것도 안하는 상태
    Patrol,     // 순찰
    Suspect,    // 의심
    Chase,      // 추적
    Attack,     // 공격
    Dead        // 사망
}

public enum PlayerState
{
    Sit = 0,
    Stand,
    Run
}

/// <summary>
/// 아이템 종류별 ID
/// </summary>
public enum ItemIDCode
{
    OldCup = 0,
    Key,
    Fuse,
    USB,
    CardKey
}

public enum ElevatorState
{
    Up = 0,
    Down,
    Stop
}