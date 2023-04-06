using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle = 0,   // �ƹ��͵� ���ϴ� ����
    Patrol,     // ����
    Suspect,    // �ǽ�
    Chase,      // ����
    Attack,     // ����
    Dead        // ���
}

public enum PlayerState
{
    Sit = 0,
    Stand,
    Run
}

/// <summary>
/// ������ ������ ID
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