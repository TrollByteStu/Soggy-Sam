using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public List<SO_Mission> _Missions;
    public GameObject _MissionPrefab;
    private void Start()
    {
        if (_MissionPrefab == null)
            Debug.LogError("no MissionPrefab assigned to " + gameObject);
    }

    public void SpawnUi()
    {
    }



}
