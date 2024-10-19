using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDelete : MonoBehaviour
{
    public GameObject Wall;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("AAAAAAAAAAAAA");
        Destroy(Wall);
    }
}
