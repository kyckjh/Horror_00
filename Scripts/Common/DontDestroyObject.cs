using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
    public static DontDestroyObject instance = null;

    private void Awake()
    {
        //if(instance != null)
        //{
        //    Destroy(this.gameObject);
        //    return;
        //}

        //instance = this;
        //DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
