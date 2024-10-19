using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public int totalpoints = 0;
    public GameObject spawn;
    void Update()
    {
        if (totalpoints == 4)
        {
            spawn.SetActive(true);
        }
    }
}
