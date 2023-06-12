using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletPhysics : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            myRB.drag = 2f;
            myRB.angularDrag = 2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            myRB.drag = 1f;
            myRB.angularDrag = 1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<predatorFish>())
        {
            collision.gameObject.GetComponent<predatorFish>().dead = true;
            Destroy(gameObject,1f);
        }
        else
        {
            Destroy(gameObject,1f);
        }

    }
}