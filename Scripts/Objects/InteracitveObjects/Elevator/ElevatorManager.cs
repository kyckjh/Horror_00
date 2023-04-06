using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    public bool isActivate = false;

    private void Awake()
    {
        room = GetComponentInChildren<ElevatorRoom>();
        elevatorBases = GetComponentsInChildren<ElevatorBase>();
    }

    ElevatorRoom room;
    public ElevatorRoom Room => room;

    ElevatorBase[] elevatorBases;
    public ElevatorBase[] ElevatorBases => elevatorBases;

    public Material screenUp, screenDown, screenUpMoving, screenDownMoving;
    public Material screenBlack, screenNum1, screenNum2, screenNum3;


}
