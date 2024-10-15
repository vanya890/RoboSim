using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingIntro : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            transform.position = new Vector3((float)Mathf.Floor(raycastHit.point.x) + 0.5f, (float)Mathf.Floor(raycastHit.point.y) + 0.5f, (float)Mathf.Floor(raycastHit.point.z) + 0.5f);
        }
    }
}
