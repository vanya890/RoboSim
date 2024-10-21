using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointGatherer : MonoBehaviour
{
    GameObject main;
    private void Awake()
    {
        main = GameObject.Find("Dots");
    }
    private void OnTriggerEnter(Collider other)
    {
        main.GetComponent<Spawner>().totalpoints++;
        Destroy(this.gameObject);
    }
}
