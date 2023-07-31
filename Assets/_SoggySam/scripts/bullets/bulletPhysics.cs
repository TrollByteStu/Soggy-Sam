using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _SoggySam.scripts.Utils;


public class bulletPhysics : WaterStateHelper
{
    private Rigidbody myRB;

    private void Start()
    {
        myRB = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        if (myRB.velocity.magnitude > 2f)
            transform.LookAt(myRB.velocity + transform.position, Vector3.up);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    protected override void OnEnterWater()
    {
        myRB.drag = 2;
        myRB.angularDrag = 2;
    }

    protected override void OnExitWater()
    {
        myRB.drag = 1;
        myRB.angularDrag = 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<predatorFish>())
        {
            collision.gameObject.GetComponent<predatorFish>().dead = true;
            Destroy(gameObject,1f);
        }
        else if (collision.gameObject.GetComponent<mobyDick>())
        {
            collision.gameObject.GetComponent<mobyDick>().DamageMoby();
            Destroy(gameObject,1f);
        }

    }
}