using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishBuoyancy : MonoBehaviour
{
    private Rigidbody myRB;
    public int inWater = 0;
    public int floaters = 1;
    public RaycastHit[] waterCheck;
    public List<GameObject> waterList = new List<GameObject>();

    void Start()
    {
        myRB = transform.parent.GetComponent<Rigidbody>();
    }

    public void dead()
    {
        waterList.Clear();
        waterCheck = Physics.RaycastAll(transform.position, Vector3.forward , 5);
        Debug.DrawRay(transform.position, Vector3.forward *5);
        Debug.Log(waterCheck.Length);
        inWater = 0;
        if (waterCheck.Length > 0)
            foreach (RaycastHit hit in waterCheck)
            {
                waterList.Add(hit.collider.gameObject);
                Debug.Log(hit.collider.name);
                Debug.DrawRay(transform.position, Vector3.forward * hit.distance);
                if (hit.collider.tag == "Water") inWater = +1;
            }
        if (inWater > 0  ) myRB.AddForceAtPosition(Vector3.up * 2, transform.position);
        else myRB.AddForceAtPosition(Physics.gravity / floaters, transform.position);

    }   
}
