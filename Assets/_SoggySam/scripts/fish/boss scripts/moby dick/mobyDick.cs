using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobyDick : MonoBehaviour
{
    public float hitpoints;
    public float speed;
    public float turn;
    public bool dead;
    public GameObject myPlayer;

    private Rigidbody myRB;
    private int currentMove;
    private bool inWater;

    void Start()
    {
        myRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        switch (currentMove)
        {
            case 0:
                if (inWater) Move();
                break;
            case 1:
                if (inWater) Charge();
                break;
            case 2:
                break;
        }
        
    }

    private bool chargeUp = true;

    private void Charge()
    {
        if (chargeUp)
        {
            transform.LookAt(myPlayer.transform);
            transform.Rotate(Vector3.up,180);
            myRB.AddForce(speed * Time.fixedDeltaTime * transform.forward);
        }
        else if (!chargeUp)
        {
            transform.LookAt(myPlayer.transform);
            myRB.AddForce(2 * speed * Time.fixedDeltaTime * transform.forward);
        }
        if (200f <= Vector3.Distance(myPlayer.transform.position, transform.position))
        {
            chargeUp = false;
        }
        else if (10f >= Vector3.Distance(myPlayer.transform.position, transform.position))
        {
            chargeUp = true;
        }
    }

    private void Move()
    {
        if (inWater && !dead)
        {
            myRB.AddForce(speed * Time.fixedDeltaTime * transform.forward);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            myRB.drag = 5;
            myRB.useGravity = false;
            inWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            myRB.drag = 1;
            myRB.useGravity = true;
            inWater = false;
        }
    }
}
