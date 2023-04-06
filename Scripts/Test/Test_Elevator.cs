using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Elevator : TestBase
{
    public ElevatorBase elevator;

    public ElevatorRoom room;

    public Transform[] destination;

    protected override void OnTest7(InputAction.CallbackContext obj)
    {
        //room.ChangeFloor(destination[0]);
    }

    protected override void OnTest8(InputAction.CallbackContext obj)
    {
        elevator.DoorOpen();
        room.DoorOpen();
        //room.ChangeFloor(destination[1]);
    }

    protected override void OnTest9(InputAction.CallbackContext obj)
    {
        elevator.DoorClose();
        room.DoorClose();
        //room.ChangeFloor(destination[2]);
    }
}
