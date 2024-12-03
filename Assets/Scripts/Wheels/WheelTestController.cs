using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTestController : MonoBehaviour
{
    public float speed = 10f;
    public float dir = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<WheelCollider>().motorTorque = (float)(Input.GetAxis("Vertical")*speed*0.5 + Input.GetAxis("Horizontal") * dir * 0.5* speed);
    }
}
