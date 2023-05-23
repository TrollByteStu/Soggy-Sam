using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universalGrenade : MonoBehaviour
{
    public Vector3 scale;
    public float mass;
    public float drag;
    public float angularDrag;
    public float aoeRadius;
    public float aoeDamage;
    public float fuseTime;

    private float spawnTime;
    private Rigidbody myRB;
    private Collider[] hits;
    private RaycastHit hitScan;


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        transform.localScale = scale;
        myRB.mass = mass;
        myRB.drag = drag;
        myRB.angularDrag = angularDrag;
        spawnTime = Time.time;
    }

    void FixedUpdate()
    {
        if (spawnTime + fuseTime <= Time.time)/// boom
        {
            hits = Physics.OverlapSphere(transform.position, aoeRadius);
            foreach (Collider hit in hits)
            {
                if (Physics.Raycast(transform.position,  hit.transform.position - transform.position, out hitScan))
                {
                    Debug.DrawRay(transform.position, hit.transform.position -transform.position, Color.red);
                    if (hit.GetComponent<playerStats>())
                    {
                        hit.GetComponent<playerStats>().health -= aoeDamage;
                    }
                    if (hit.GetComponent<predatorFish>())
                    {
                        hit.GetComponent<predatorFish>().dead = true;
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
