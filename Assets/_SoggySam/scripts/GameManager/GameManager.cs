using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public GameObject player;
    public MissionManager missionManager;
    public playerStats stats;
    public Camera _MainCamera;
    public cameraController _MainCameraScript;

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
        if (!_MainCamera)
            _MainCamera = FindObjectOfType<Camera>();
        if (!_MainCameraScript)
            _MainCameraScript = _MainCamera.GetComponent<cameraController>();
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
