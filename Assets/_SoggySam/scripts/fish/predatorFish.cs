using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class predatorFish : MonoBehaviour
{
    private Rigidbody myRB;
    private RaycastHit myEyes;
    private RaycastHit myMouth;
    private LayerMask waterMask;
    [Header("fight, flight, fuck")]

    public SO_Item SoLoot;
    public string species;
    public List<predatorFish> preyList;
    public List<predatorFish> predatorList;

    public GameObject myPlayer;
    public float hungry = 0;

    public int FishTier = 1;
    public float swimSpeed = 10;
    public float turnSpeed = 10;
    public float sight = 5;
    public GameObject prefab;
    private Collider[] unfish;
    private List<GameObject> dangerFish;
    private List<GameObject> foodFish;
    private List<GameObject> breedFish;
    private fishBuoyancy[] buoyancy;
    public bool dead = false;
    public bool inWater = true;
    public bool harpooned = false;

    public Quaternion direction;

    private const float zOffset = -0; // fish might now need to be at z0 in later updates ajust here in that case

    private void Start()
    {
        waterMask = LayerMask.GetMask("Water");
        GameObject[] gameObjects = { gameObject };
        myRB = GetComponent<Rigidbody>();
        dangerFish = new List<GameObject>(gameObjects);
        foodFish = new List<GameObject>(gameObjects);
        breedFish = new List<GameObject>(gameObjects);
        buoyancy = transform.GetComponentsInChildren<fishBuoyancy>();
    }
    void FixedUpdate()
    {
        if (!dead)
        {
            if (FishTier > 0) /// makes fish hungry unless its at the bottom of the foodchain
                hungry -= Time.deltaTime;
            else
                hungry += Time.deltaTime;
            if (hungry <= -300) dead = true;
        }
        if (!dead && inWater)
        {
            fishSight(); /// sees all fish within sight that are not blocked by a wall
            sortFish(); /// sorts fish by distance
            survivalChoice(); /// chooses betwen run, eat or breed and then turns the turntransform 
            transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, zOffset) ///puts fish back to z0.5
                , Quaternion.Lerp(transform.rotation, direction, Time.fixedDeltaTime * 0.1f * turnSpeed)); /// rotates fish
            Vector3 stay2D = new (transform.forward.x, transform.forward.y, zOffset);
            myRB.AddForce(stay2D * swimSpeed);

        }
        else if (dead || !inWater)
        {
            foreach (fishBuoyancy boy in buoyancy)
            {
                boy.dead();
            }
        }
    }
    private void fishSight()
    {
        unfish = Physics.OverlapSphere(transform.position, sight);
        dangerFish.Clear();
        foodFish.Clear();
        breedFish.Clear();
        foreach (Collider col in unfish)
        {
            if (Physics.Raycast(transform.position, col.transform.position - transform.position, out myEyes) && col.gameObject.GetComponent<predatorFish>() || col.gameObject.GetComponent<landController>())
            {
                if (myEyes.collider.gameObject == col.gameObject)
                {
                    if (col.gameObject == gameObject) { }
                    if (col.GetComponent<predatorFish>())
                    {
                        predatorFish script = col.GetComponent<predatorFish>();
                        foreach (predatorFish prey in preyList)
                            if (prey.species == script.species)
                                foodFish.Add(col.gameObject);
                        foreach (predatorFish predator in predatorList)
                            if (predator.species == script.species)
                                dangerFish.Add(col.gameObject);
                        if (species == script.species && script.hungry >= 0)
                            breedFish.Add(col.gameObject);
                    }
                    else if (col.GetComponent<playerStats>() && FishTier > 0)
                        foodFish.Add(col.gameObject);
                }
                else
                    Debug.DrawRay(transform.position, col.transform.position - transform.position, Color.white);
            }
        }
    }
    private void sortFish()
    {
        foodFish.Sort((t2, t1) => Vector3.Distance(gameObject.transform.position, t2.transform.position).CompareTo(Vector3.Distance(gameObject.transform.position, t1.transform.position)));
        dangerFish.Sort((t2, t1) => Vector3.Distance(gameObject.transform.position, t2.transform.position).CompareTo(Vector3.Distance(gameObject.transform.position, t1.transform.position)));
        breedFish.Sort((t2, t1) => Vector3.Distance(gameObject.transform.position, t2.transform.position).CompareTo(Vector3.Distance(gameObject.transform.position, t1.transform.position)));
    }
    private void survivalChoice()
    {

        if (dangerFish.Count > 0) /// avoid predator
        {
            //Debug.Log("danger");
            if (Physics.Raycast(transform.position, transform.forward, out myEyes, 10f)) // avoid obstacles
            {
                Debug.DrawRay(transform.position, transform.forward * myEyes.distance, Color.cyan);
                transform.Rotate(-Vector3.right * Time.fixedDeltaTime * 200);
                direction = transform.rotation;
            }
            else if (Physics.Raycast(transform.position, dangerFish[0].transform.position - transform.position, out myEyes))
                Debug.DrawRay(transform.position, dangerFish[0].transform.position - transform.position, Color.red);

            direction = Quaternion.LookRotation(transform.position - dangerFish[0].transform.position,Vector3.up);
        }
        else if (breedFish.Count > 0 && hungry > 0) /// breed
        {
            if (Physics.Raycast(transform.position, breedFish[0].transform.position - transform.position, out myEyes))
                Debug.DrawRay(transform.position, breedFish[0].transform.position - transform.position, Color.yellow);

           direction = Quaternion.LookRotation(breedFish[0].transform.position - transform.position,Vector3.up);

            if (Physics.Raycast(transform.position, transform.forward, out myMouth, 1f))
            {
                if (myMouth.collider.gameObject == breedFish[0].gameObject && breedFish[0].GetComponent<predatorFish>().hungry > 0)
                {
                    hungry -= 30 * (FishTier + 1);
                    myMouth.collider.GetComponent<predatorFish>().hungry -= 30 * (FishTier + 1);
                    Instantiate(prefab);
                }

            }

        }
        else if (foodFish.Count > 0 && hungry < 0) /// hunt food
        {
            if (Physics.Raycast(transform.position, foodFish[0].transform.position - transform.position, out myEyes))
                Debug.DrawRay(transform.position, foodFish[0].transform.position - transform.position, Color.green);
            direction = Quaternion.LookRotation(foodFish[0].transform.position - transform.position, Vector3.up);
            if (Physics.Raycast(transform.position, transform.forward, out myMouth, 1.5f))
            {
                if (myMouth.collider.gameObject == foodFish[0])
                {
                    if (myMouth.collider.GetComponent<predatorFish>()) /// eat fish
                    {
                        hungry += 30 * (myMouth.collider.GetComponent<predatorFish>().FishTier + 1);
                        myRB.AddForce(transform.forward * swimSpeed * 10f);
                        if (myMouth.collider.GetComponent<predatorFish>().harpooned)
                        {
                            harpooned = true;
                            myPlayer = myMouth.collider.GetComponent<predatorFish>().myPlayer;
                            myPlayer.GetComponent<SpringJoint>().connectedBody = myRB;
                        }
                        Destroy(foodFish[0].gameObject, 0.1f);
                    }
                    else if (myMouth.collider.GetComponent<playerStats>()) /// eat player
                    {
                        var stats = myMouth.collider.GetComponent<playerStats>();
                        if (stats.invulnerable < Time.time)
                        {
                            stats.invulnerable = Time.time + stats.invulnerableTime;
                            stats.health--;
                            myMouth.collider.GetComponent<Rigidbody>().AddForce(-80 * (transform.position - myMouth.collider.transform.position));
                        }

                    }
                }
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward, out myEyes, 10f, ~waterMask)) /// avoid obstacles
        {
            Debug.DrawRay(transform.position, transform.forward * myEyes.distance, Color.cyan);
            transform.Rotate(200 * Time.fixedDeltaTime * -Vector3.right);
            direction = transform.rotation;
        } else
        { /// idle

            Vector3 stay2D = new (transform.forward.x + transform.position.x, transform.forward.y + transform.position.y, zOffset);
            transform.LookAt(stay2D);
            transform.Rotate(Vector3.right, Mathf.Cos(Time.fixedTime * 1f+ Mathf.PerlinNoise(transform.position.x,transform.position.y))*Time.deltaTime*10f);
            direction = transform.rotation;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = true;
            if (myRB)
                myRB.useGravity = false;
            else
                myRB = GetComponent<Rigidbody>();
            myRB.drag = 5f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (myRB)
                myRB.useGravity = true;
            else
                myRB = GetComponent<Rigidbody>();
            myRB.drag = 1;
            inWater = false;
            transform.forward *= -1;
            direction = transform.rotation;
            Vector3 stay2D = new (transform.forward.x, transform.forward.y, 0);
            myRB.AddForce(10 * swimSpeed * stay2D);
            transform.position += stay2D * 1.5f;
        }
    }
}
