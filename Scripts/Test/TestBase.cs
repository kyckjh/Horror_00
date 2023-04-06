using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBase : MonoBehaviour
{
    PlayerInputActions inputActions;

    protected virtual void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Test.Enable();
        //inputActions.Test.Click.performed += OnTestClick;
        inputActions.Test.Test9.performed += OnTest9;
        inputActions.Test.Test8.performed += OnTest8;
        inputActions.Test.Test7.performed += OnTest7;
        inputActions.Test.Test6.performed += OnTest6;
    }
    private void OnDisable()
    {
        inputActions.Test.Test6.performed -= OnTest6;
        inputActions.Test.Test7.performed -= OnTest7;
        inputActions.Test.Test8.performed -= OnTest8;
        inputActions.Test.Test9.performed -= OnTest9;
        //inputActions.Test.Click.performed -= OnTestClick;
        inputActions.Test.Disable();
    }

    protected virtual void OnTest9(InputAction.CallbackContext obj)
    {
    }

    protected virtual void OnTest8(InputAction.CallbackContext obj)
    {
    }

    protected virtual void OnTest7(InputAction.CallbackContext obj)
    {
    }

    protected virtual void OnTest6(InputAction.CallbackContext obj)
    {
    }
    //protected virtual void OnTestClick(InputAction.CallbackContext obj)
    //{
    //}
}
