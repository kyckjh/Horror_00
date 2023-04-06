using System.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Screen : TestBase
{


    void Start()
    {
        //Debug.Log($"{}");
        foreach (var res in Screen.resolutions)
        {
            Debug.Log($"{res}");
        }
    }

    protected override void OnTest7(InputAction.CallbackContext obj)
    {
        Resolution res = Screen.currentResolution;
        Screen.SetResolution(res.width, res.height, true);
    }

    protected override void OnTest8(InputAction.CallbackContext obj)
    {
        Resolution res = Screen.resolutions[Screen.resolutions.Length - 1];
        Screen.SetResolution(640, 640, FullScreenMode.FullScreenWindow);
    }

    protected override void OnTest9(InputAction.CallbackContext obj)
    {
        Resolution res = Screen.currentResolution;
        Screen.SetResolution(res.width, res.height, false);
    }
}


