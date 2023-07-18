using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _SoggySam.scripts.Utils;

public class universalGrenade : WaterStateHelper
{
    public Vector3 scale;
    public float mass;
    public float drag;
    public float angularDrag;
    public float WaterDrag;
    public float WaterAngularDrag;
    public float aoeRadius;
    public float aoeDamage;
    public float fuseTime;

    private float spawnTime;
    private Rigidbody myRB;
    private Collider[] hits;
    private RaycastHit hitScan;

    [SerializeField] private GameObject ExplosionPrefab;

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

    void FixedUpdate()
    {
        if (spawnTime + fuseTime <= Time.time)/// boom
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
            hits = Physics.OverlapSphere(transform.position, aoeRadius);
            foreach (Collider hit in hits)
            {
                if (Physics.Raycast(transform.position,  hit.transform.position - transform.position, out hitScan))
                {
                    Debug.DrawRay(transform.position, hit.transform.position -transform.position, Color.red);
                    if (hit.GetComponent<playerStats>())
                    {
                        hit.GetComponent<playerStats>()._CurrentHealth -= aoeDamage;
                    }
                    if (hit.GetComponent<predatorFish>())
                    {
                        hit.GetComponent<predatorFish>().dead = true;
                    }
                    if (hit.GetComponent<mobyDick>())
                    {
                        hit.GetComponent<mobyDick>().DamageMoby(aoeDamage);
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
