using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BE2_VirtualJoystick : MonoBehaviour
{
    public static BE2_VirtualJoystick instance;
    public BE2_VirtualJoystickButton[] keys = new BE2_VirtualJoystickButton[6];

    void Awake()
    {
        instance = this;

        int i = 0;
        foreach (Transform child in transform.GetChild(0))
        {
            BE2_VirtualJoystickButton key = child.GetComponent<BE2_VirtualJoystickButton>();
            if (key != null)
            {
                keys[i] = key;
                i++;
            }
        }
    }

    //void Start()
    //{
    //
    //}
    //
    //void Update()
    //{
    //    
    //}
}