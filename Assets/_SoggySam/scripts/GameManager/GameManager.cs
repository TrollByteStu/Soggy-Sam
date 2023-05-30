using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject player;
    public playerStats stats;

    public void Start()
    {
        if (!stats)
        stats = player.GetComponent<playerStats>();
    }


}
