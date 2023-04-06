using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_UI : TestBase
{
    protected override void OnTest9(InputAction.CallbackContext obj)
    {
        UIManager.Inst.SetMessagePanel("Test Å×½ºÆ®");
    }

}
