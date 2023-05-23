using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class harpoonPhysics : MonoBehaviour
{
    private Collider myCollider;
    public GameObject myPlayer;
    private Rigidbody myRB;

    private const float zOffset = -0; // fish might now need to be at z0 in later updates ajust here in that case

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        myRB = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        if (myRB.velocity.magnitude > 2f)
        transform.LookAt(myRB.velocity + transform.position, Vector3.up);
        transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            myRB.drag = 2f;
            myRB.angularDrag = 2f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            myRB.drag = 1f;
            myRB.angularDrag = 1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<predatorFish>())
        {
            predatorFish predator = collision.gameObject.GetComponent<predatorFish>();
            predator.dead = true;
            predator.harpooned = true;
            predator.myPlayer = myPlayer;
            myPlayer.GetComponent<SpringJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            transform.parent = collision.gameObject.transform;
            myRB.isKinematic = true;
            myCollider.enabled = false;

        }
        if (collision.gameObject.GetComponent<weaponController>())
        {
            collision.gameObject.GetComponent<weaponController>().readyshot = true;
            collision.gameObject.GetComponent<weaponController>().weapon.SetActive(true);
            myPlayer.GetComponent<SpringJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            Destroy(gameObject);
        }
        else
        {
            transform.parent = collision.gameObject.transform;
            myRB.isKinematic = true;
        }

    }
}
