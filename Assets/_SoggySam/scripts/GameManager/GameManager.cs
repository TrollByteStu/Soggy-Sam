using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public GameObject player;
    public playerStats stats;

    public void Awake()
    {
        _instance = this;
        if (!stats && player)
            stats = player.GetComponent<playerStats>();
    }

    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Game manager is NULL");
            return _instance;
        }
    }
    
}
