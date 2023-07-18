using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using _SoggySam.scripts.Utils;

public class harpoonPhysics : WaterStateHelper
{
    private Collider myCollider;
    public GameObject myPlayer;
    private Rigidbody myRB;

    public Vector3 scale;
    public float mass;
    public float drag;
    public float angularDrag;
    public float WaterDrag;
    public float WaterAngularDrag;
    public bool ReloadOnPickup;

    private const float zOffset = -0; // fish might now need to be at z0 in later updates ajust here in that case

    public int Damage;
    public bool SpringJoint = false;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        myRB = GetComponent<Rigidbody>();

        transform.localScale = scale;
        myRB.mass = mass;
        myRB.drag = drag;
        myRB.angularDrag = angularDrag;
    }


    private void FixedUpdate()
    {
        if (myRB.velocity.magnitude > 2f)
        transform.LookAt(myRB.velocity + transform.position, Vector3.up);
        transform.position = new Vector3(transform.position.x, transform.position.y, zOffset);
    }

    protected override void OnEnterWater()
    {
        myRB.drag = WaterDrag;
        myRB.angularDrag = WaterAngularDrag;
    }

    protected override void OnExitWater()
    {
        myRB.drag = drag;
        myRB.angularDrag = angularDrag; 
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<weaponController>() && ReloadOnPickup)
        {
            collision.gameObject.GetComponent<weaponController>()._Readyshot = true;
            collision.gameObject.GetComponent<weaponController>()._Weapon.SetActive(true);
            if (myPlayer.GetComponent<SpringJoint>())
                myPlayer.GetComponent<SpringJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponent<predatorFish>())
        {
            predatorFish predator = collision.gameObject.GetComponent<predatorFish>();
            predator.dead = true;
            predator.harpooned = true;
            predator.myPlayer = myPlayer;
            if (SpringJoint)
                myPlayer.GetComponent<SpringJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            transform.parent = collision.gameObject.transform;
            myRB.isKinematic = true;
            myCollider.enabled = false;

        }
        else if (collision.gameObject.GetComponent<mobyDick>())
        {
            mobyDick moby = collision.gameObject.GetComponent<mobyDick>();
            moby.DamageMoby(Damage);
            if (SpringJoint)
                myPlayer.GetComponent<SpringJoint>().connectedBody = collision.gameObject.GetComponent<Rigidbody>();
            transform.parent = collision.gameObject.transform;
            myRB.isKinematic = true;
            myCollider.isTrigger = true;

        }
        else
        {
            transform.parent = collision.gameObject.transform;
            myRB.isKinematic = true;
        }

    }

}
