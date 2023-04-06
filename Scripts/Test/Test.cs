using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ItemFactory.MakeItem(ItemIDCode.Fuse, transform.position);
        ItemFactory.MakeItem(ItemIDCode.OldCup, transform.position + transform.up * 0.5f);
        ItemFactory.MakeItem(ItemIDCode.Key, transform.position + transform.up);
    }

}
