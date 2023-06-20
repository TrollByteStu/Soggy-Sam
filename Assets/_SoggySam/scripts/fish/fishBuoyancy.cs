using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishBuoyancy : MonoBehaviour
{
    private Rigidbody myRB;
    public int Floaters = 1;
    public bool InWater;
    public Collider[] WaterArray;

    void Start()
    {
        myRB = transform.GetComponentInParent<Rigidbody>();
    }

    public void dead()
    {
        EnactPhysics();
    }

    public void EnactPhysics()
    {
        if (CheckWater()) myRB.AddForceAtPosition(Vector3.up * (myRB.mass / 2), transform.position);
        else myRB.AddForceAtPosition((Physics.gravity / Floaters) * (myRB.mass / 2), transform.position);
    }

    public bool CheckWater()
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
        return InWater;
    }
}
