using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buoyancy : MonoBehaviour
{
    private Rigidbody myRB;
    private RaycastHit water;
    public int floaters = 1;

    void Start()
    {
        myRB = transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        
        Physics.Raycast(transform.position, Vector3.forward, out water);
        if (water.collider != null)
        {
            if (water.collider.tag == "Water")
            {
                myRB.AddForceAtPosition(Vector3.up * (myRB.mass / 2), transform.position);
            }
        }
        else
        {
            myRB.AddForceAtPosition((Physics.gravity / floaters) * (myRB.mass / 2), transform.position);
        }
    }
}
