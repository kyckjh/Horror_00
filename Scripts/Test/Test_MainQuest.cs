using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_MainQuest : TestBase
{
    public Desktop desktop;

    public Transform itemPos;

    private void Start()
    {
        ItemFactory.MakeItem(ItemIDCode.USB, itemPos.position);
        ItemFactory.MakeItem(ItemIDCode.Fuse, itemPos.position + Vector3.right);
    }

    protected override void OnTest9(InputAction.CallbackContext obj)
    {
        ItemData_USB usb = new ItemData_USB();
        
        usb.id = (uint)ItemIDCode.USB;
        desktop.Set(usb);
    }

}
