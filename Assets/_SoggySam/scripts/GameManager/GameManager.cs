using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public MissionManager missionManager;
    public HudManager _HudManager;
    public GameObject player;
    public playerStats stats;
    public Camera _MainCamera;
    public cameraController _MainCameraScript;

    // is the ui busy with animation?
    public bool uiBusy = false;

    // calls ui popup from the central game manager
    public void callUiPopup(int i)
    {
        // we are busy
        uiBusy = true;

        // show the actual popup
        _HudManager.myGameStateUI.showPopup(i);
    }

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
        if (GetComponent<HudManager>())
            _HudManager = GetComponent<HudManager>();
        else
        {
            gameObject.AddComponent<HudManager>();
            _HudManager = GetComponent<HudManager>();
        }
        if (!stats && player)
            stats = player.GetComponent<playerStats>();
        if (!_MainCamera)
            _MainCamera = FindObjectOfType<Camera>();
        if (!_MainCameraScript)
            _MainCameraScript = _MainCamera.GetComponent<cameraController>();
    }

    private void Update()
    {
        if (stats._CurrentHealth <= 0f && !uiBusy)
        {
            _HudManager.myGameStateUI.showPopup(0);
        }
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
