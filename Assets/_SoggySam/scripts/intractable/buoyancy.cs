using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buoyancy : MonoBehaviour
{
    private Rigidbody myRB;
    public int Floaters = 1;
    public Collider[] WaterArray;
    public bool InWater = false;

    void Start()
    {
        myRB = transform.parent.GetComponent<Rigidbody>();
    }

    void CheckWater()
    {
        InWater = false;
        WaterArray = Physics.OverlapBox(transform.position, Vector3.forward);
        if (WaterArray.Length != 0)
        {
            foreach (Collider hit in WaterArray)
            {
                if (hit.tag == "Water")
                {
                    InWater = true;
                }
            }

        }
    }

    void EnactPhysics()
    {
        if (InWater) myRB.AddForceAtPosition(Vector3.up * (myRB.mass / 2) , transform.position);
        else myRB.AddForceAtPosition((Physics.gravity / Floaters) * (myRB.mass / 2), transform.position);
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        CheckWater();
        EnactPhysics();
    }
}
