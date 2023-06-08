using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public MissionManager missionManager;
    public GameObject player;
    public playerStats stats;

    public void Awake()
    {
        _instance = this;
        if (GetComponent<MissionManager>())
            missionManager = GetComponent<MissionManager>();
        else
        {
            gameObject.AddComponent<MissionManager>();
            missionManager = GetComponent<MissionManager>();
        }
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
