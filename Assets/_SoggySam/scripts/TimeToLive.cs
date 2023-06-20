using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToLive : MonoBehaviour
{
    [SerializeField] private float secondsToLive = 5f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, secondsToLive);
    }

}
