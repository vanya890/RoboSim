using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDistanceSensor : MonoBehaviour
{
    public float maxDistance = 10f; // Максимальная дистанция обнаружения
    public LayerMask layerMask; // Слои, которые будут обнаруживаться

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance, layerMask))
        {
            Debug.Log("Distance to obstacle: " + hit.distance);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red); // Визуализация луча
        }
        else
        {
            Debug.Log("No obstacle detected within " + maxDistance + " units.");
            Debug.DrawRay(transform.position, transform.forward * maxDistance, Color.green); // Визуализация луча
        }
    }
}
